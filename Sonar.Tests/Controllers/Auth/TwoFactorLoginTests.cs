using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.Utilities;
using Entities.Enums;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Sonar.Controllers;
using Sonar.Controllers.UserCore;
using Sonar.Middleware;
using Sonar.Services.Authentication;
using Sonar.Tests.Controllers.Chat;
using Xunit;

namespace Sonar.Tests.Controllers.Auth;

public class TwoFactorLoginTests : ChatControllerTestsBase, IDisposable
{
    private readonly SonarContext db;
    private readonly UserManager<User> manager;
    private readonly TestServer server;
    private readonly HttpClient client;
    private readonly User account;
    private readonly List<UserSession> sessions = [];
    private readonly TestClock clock = new();
    private string? sentCode;
    private const string Password = "Fixture-only-correct-password";

    public TwoFactorLoginTests()
    {
        db = CreateInMemoryContext();
        account = CreateTestUser(17);
        account.EmailConfirmed = true;
        account.Enabled2FA = true;
        account.SecurityStamp = "fixture-stamp";
        db.Users.Add(account);
        db.SaveChanges();
        manager = CreateUserManager(db);
        var signIn = new Mock<SignInManager<User>>(manager, new HttpContextAccessor(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(), Options.Create(new IdentityOptions()),
            Mock.Of<ILogger<SignInManager<User>>>(), Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<User>>());
        signIn.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), false))
            .ReturnsAsync((User _, string password, bool _) => password == Password
                ? SignInResult.Success : SignInResult.Failed);
        var mail = new Mock<IEmailSenderService>(MockBehavior.Strict);
        mail.Setup(x => x.SendEmailAsync(account.Email!, MailTemplates.twoFA, It.IsAny<Dictionary<string, string>>()))
            .Callback((string _, string _, Dictionary<string, string> variables) => sentCode = variables["code"])
            .Returns(Task.CompletedTask);
        var sessionService = new Mock<IUserSessionService>(MockBehavior.Strict);
        sessionService.Setup(x => x.CreateAsync(It.IsAny<UserSession>()))
            .ReturnsAsync((UserSession session) => { session.Id = 901; sessions.Add(session); return session; });
        var userService = Mock.Of<IUserService>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "fictional-test-signing-key-at-least-thirty-two-bytes",
            ["Jwt:Issuer"] = "fixture", ["Jwt:Audience"] = "fixture"
        }).Build();
        // Isolated HTTP host: no production startup, migrations, network mail or database.
        server = new TestServer(new WebHostBuilder().UseEnvironment("Development")
            .ConfigureServices(services =>
            {
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton(manager);
                services.AddSingleton(signIn.Object);
                services.AddSingleton(userService);
                services.AddSingleton(mail.Object);
                services.AddSingleton(sessionService.Object);
                services.AddSingleton(new AuthService(config, userService));
                services.AddSingleton(new TwoFactorChallengeStore(clock));
                services.AddAuthentication("fixture").AddScheme<AuthenticationSchemeOptions, AnonymousHandler>("fixture", _ => { });
                services.AddAuthorization();
                services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }));
        client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Device-Name", "initial-device");
    }

    [Fact]
    public async Task FreshClientCompletesTwoFactorLoginWithoutBearerTokenAndCannotReplay()
    {
        var challenge = await LoginChallenge();
        Assert.Empty(sessions);
        Assert.False(challenge.TryGetProperty("accessToken", out _));
        Assert.False(challenge.TryGetProperty("refreshToken", out _));
        client.DefaultRequestHeaders.Remove("X-Device-Name");
        client.DefaultRequestHeaders.Add("X-Device-Name", "changed-device");
        var payload = new { challengeToken = challenge.GetProperty("challengeToken").GetString(), code = sentCode };
        var response = await client.PostAsJsonAsync("/api/auth/verify-2fa", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(data.GetProperty("accessToken").GetString());
        Assert.Equal("17", token.Subject);
        Assert.False(string.IsNullOrEmpty(data.GetProperty("refreshToken").GetString()));
        Assert.Equal(901, data.GetProperty("sessionId").GetInt32());
        var session = Assert.Single(sessions);
        Assert.Equal(17, session.UserId);
        Assert.Equal("initial-device", session.DeviceName);
        Assert.NotEqual(data.GetProperty("refreshToken").GetString(), session.RefreshTokenHash);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/verify-2fa", payload)).StatusCode);
        Assert.Single(sessions);
    }

    [Fact]
    public async Task IncorrectPasswordIssuesNeitherChallengeNorSession()
    {
        var response = await client.PostAsync("/api/auth/login?userIdentifier=testuser17&password=wrong", null);
        Assert.Equal(HttpStatusCode.ExpectationFailed, response.StatusCode);
        Assert.Null(sentCode);
        Assert.Empty(sessions);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("userId")]
    public async Task VerificationRejectsClientSelectedIdentity(string field)
    {
        var challenge = await LoginChallenge();
        var payload = new Dictionary<string, object?>
        {
            ["challengeToken"] = challenge.GetProperty("challengeToken").GetString(), ["code"] = sentCode,
            [field] = field == "email" ? "outsider@example.invalid" : 999
        };
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/auth/verify-2fa", payload)).StatusCode);
        Assert.Empty(sessions);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("security-stamp")]
    [InlineData("email")]
    [InlineData("disabled")]
    [InlineData("permission")]
    [InlineData("unconfirmed")]
    [InlineData("locked")]
    [InlineData("deleted")]
    public async Task VerificationRechecksChallengeAndAccountState(string change)
    {
        var challenge = await LoginChallenge();
        switch (change)
        {
            case "expired": clock.Now += TwoFactorChallengeStore.Lifetime; break;
            case "security-stamp": account.SecurityStamp = "changed-stamp"; break;
            case "email": account.Email = "changed@example.invalid"; break;
            case "disabled": account.Enabled2FA = false; break;
            case "permission": account.AccessFeatures.Clear(); break;
            case "unconfirmed": account.EmailConfirmed = false; break;
            case "locked": account.LockoutEnabled = true; account.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(10); break;
            case "deleted": db.Users.Remove(account); break;
        }
        await db.SaveChangesAsync();
        var response = await client.PostAsJsonAsync("/api/auth/verify-2fa", new
        {
            challengeToken = challenge.GetProperty("challengeToken").GetString(), code = sentCode
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(sessions);
    }

    [Fact]
    public async Task EmailCodeWithoutItsPasswordVerifiedChallengeCannotCreateSession()
    {
        await LoginChallenge();
        var response = await client.PostAsJsonAsync("/api/auth/verify-2fa", new
        {
            challengeToken = new string('x', 43), code = sentCode
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(sessions);
    }

    [Fact]
    public async Task OrdinaryLoginStillReturnsCompleteSession()
    {
        account.Enabled2FA = false;
        await db.SaveChangesAsync();
        var response = await client.PostAsync($"/api/auth/login?userIdentifier=testuser17&password={Password}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data");
        Assert.True(data.TryGetProperty("accessToken", out _));
        Assert.Single(sessions);
        Assert.Null(sentCode);
    }

    private async Task<JsonElement> LoginChallenge()
    {
        var response = await client.PostAsync($"/api/auth/login?userIdentifier=testuser17&password={Password}", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data");
        Assert.True(data.GetProperty("requiresTwoFactor").GetBoolean());
        Assert.NotNull(sentCode);
        return data;
    }

    public void Dispose() { client.Dispose(); server.Dispose(); manager.Dispose(); db.Dispose(); }

    private sealed class TestClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class AnonymousHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());
    }
}
