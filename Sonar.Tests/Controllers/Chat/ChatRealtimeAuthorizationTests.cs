using System.Reflection;
using Application.Abstractions.Interfaces.Repository.Chat;
using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.Response;
using Application.Services.Chat;
using Entities.Enums;
using Entities.Models.Access;
using Entities.Models.File;
using Entities.Models.UserCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sonar.Controllers.Test;
using Sonar.Hubs;
using Sonar.Tests.Services.Chat;
using Xunit;
using ChatModel = Entities.Models.Chat.Chat;

namespace Sonar.Tests.Controllers.Chat;

public sealed class ChatRealtimeAuthorizationTests : ChatControllerTestsBase
{
    [Fact]
    public async Task OutsiderCannotSubscribeOrSendTypingToAChat()
    {
        using var fixture = await CreateFixture();
        await fixture.CreateHub(fixture.Owner, "owner").JoinChat(fixture.Chat.Id);
        var outsider = fixture.CreateHub(fixture.Outsider, "outsider");

        await Assert.ThrowsAsync<ForbiddenResponse>(() => outsider.JoinChat(fixture.Chat.Id));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => outsider.Typing(fixture.Chat.Id));

        Assert.Empty(fixture.Transport.GroupsFor("outsider"));
        Assert.Empty(fixture.Transport.Deliveries);
    }

    [Fact]
    public async Task MemberSubscriptionsAreIsolatedAndTypingReachesOtherMembers()
    {
        using var fixture = await CreateFixture();
        await fixture.CreateHub(fixture.Owner, "owner").JoinChat(fixture.Chat.Id);
        var member = fixture.CreateHub(fixture.Member, "member-desktop");
        await member.JoinChat(fixture.Chat.Id);
        await fixture.CreateHub(fixture.Member, "member-mobile").JoinChat(fixture.Chat.Id);

        Assert.Equal(fixture.Transport.GroupsFor("member-desktop"), fixture.Transport.GroupsFor("member-mobile"));
        Assert.NotEqual(fixture.Transport.GroupsFor("owner").Single(), fixture.Transport.GroupsFor("member-desktop").Single());

        await member.Typing(fixture.Chat.Id);

        var delivery = Assert.Single(fixture.Transport.Deliveries);
        Assert.Equal("Typing", delivery.Name);
        Assert.Equal(new[] { "owner" }, delivery.Connections);
    }

    [Fact]
    public async Task SubscriptionToAnotherChatDoesNotReceiveThisChatsEvents()
    {
        using var fixture = await CreateFixture();
        var otherChat = new ChatModel
        {
            Id = 702, Name = "Other group", IsGroup = true,
            CreatorId = fixture.Owner.Id, Creator = fixture.Owner,
            CoverId = fixture.Chat.CoverId, Cover = fixture.Chat.Cover,
            Members = new List<User> { fixture.Owner, fixture.Outsider }, Admins = [], Messages = []
        };
        fixture.Context.Chats.Add(otherChat);
        await fixture.Context.SaveChangesAsync();
        await fixture.CreateHub(fixture.Owner, "owner-other-chat").JoinChat(otherChat.Id);
        await fixture.CreateHub(fixture.Member, "member-this-chat").JoinChat(fixture.Chat.Id);

        await fixture.Notifier.MessageCreated(NewMessage(fixture.Chat.Id));

        Assert.Equal(new[] { "member-this-chat" }, Assert.Single(fixture.Transport.Deliveries).Connections);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RemovedOrDepartedMemberWithExistingConnectionsReceivesNoLaterContent(bool leavesVoluntarily)
    {
        using var fixture = await CreateFixture();
        await fixture.CreateHub(fixture.Owner, "owner").JoinChat(fixture.Chat.Id);
        var desktop = fixture.CreateHub(fixture.Member, "member-desktop");
        await desktop.JoinChat(fixture.Chat.Id);
        await fixture.CreateHub(fixture.Member, "member-mobile").JoinChat(fixture.Chat.Id);

        await fixture.Notifier.MessageCreated(NewMessage(fixture.Chat.Id));
        Assert.Equal(new[] { "member-desktop", "member-mobile", "owner" },
            Assert.Single(fixture.Transport.Deliveries).Connections);
        fixture.Transport.Deliveries.Clear();

        if (leavesVoluntarily)
            await fixture.Service.LeaveChat(fixture.Member.Id, fixture.Chat.Id);
        else
            await fixture.Service.RemoveUserFromChat(fixture.Owner.Id, fixture.Member.Id, fixture.Chat.Id);

        var removal = Assert.Single(fixture.Transport.Deliveries);
        Assert.Equal("chat.user.removed", removal.Name);
        Assert.Equal(new[] { "member-desktop", "member-mobile", "owner" }, removal.Connections);
        Assert.DoesNotContain(fixture.Member.Id, await fixture.Context.Chats
            .Where(chat => chat.Id == fixture.Chat.Id).SelectMany(chat => chat.Members.Select(user => user.Id)).ToListAsync());

        // Keep both old connections subscribed: authorization must not depend on
        // cooperative clients processing the removal event or disconnecting.
        Assert.NotEmpty(fixture.Transport.GroupsFor("member-desktop"));
        Assert.NotEmpty(fixture.Transport.GroupsFor("member-mobile"));
        fixture.Transport.Deliveries.Clear();

        await fixture.Notifier.MessageCreated(NewMessage(fixture.Chat.Id));
        await fixture.Notifier.MessageUpdated(new(fixture.Chat.Id, 101, fixture.Owner.Id, "changed", DateTime.UtcNow));
        await fixture.Notifier.MessageDeleted(new(fixture.Chat.Id, 101, fixture.Owner.Id));
        await fixture.Notifier.MessagesRead(new(fixture.Chat.Id, fixture.Owner.Id, new[] { 101 }, DateTime.UtcNow));
        await fixture.Notifier.ChatNameUpdated(new(fixture.Chat.Id, "renamed"));
        await fixture.Notifier.ChatCoverUpdated(new(fixture.Chat.Id, 9));
        await fixture.Notifier.UserAdded(new(fixture.Chat.Id, fixture.Owner.Id));

        Assert.Equal(7, fixture.Transport.Deliveries.Count);
        Assert.All(fixture.Transport.Deliveries, delivery => Assert.Equal(new[] { "owner" }, delivery.Connections));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => desktop.JoinChat(fixture.Chat.Id));
        await Assert.ThrowsAsync<ForbiddenResponse>(() => desktop.Typing(fixture.Chat.Id));

        // Unsubscribing remains possible after membership has been revoked.
        await desktop.LeaveChat(fixture.Chat.Id);
        Assert.Empty(fixture.Transport.GroupsFor("member-desktop"));
    }

    [Fact]
    public async Task DeletedChatIsAnnouncedToThePreviousMembersOnly()
    {
        using var fixture = await CreateFixture();
        await fixture.CreateHub(fixture.Owner, "owner").JoinChat(fixture.Chat.Id);
        await fixture.CreateHub(fixture.Member, "member").JoinChat(fixture.Chat.Id);

        await fixture.Service.DeleteChatAsync(fixture.Owner.Id, fixture.Chat.Id);

        Assert.False(await fixture.Context.Chats.AnyAsync(chat => chat.Id == fixture.Chat.Id));
        var deleted = Assert.Single(fixture.Transport.Deliveries);
        Assert.Equal("chat.deleted", deleted.Name);
        Assert.Equal(new[] { "member", "owner" }, deleted.Connections);
        fixture.Transport.Deliveries.Clear();
        await fixture.Notifier.MessageCreated(NewMessage(fixture.Chat.Id));
        Assert.Empty(fixture.Transport.Deliveries);
    }

    private static MessageCreatedEvent NewMessage(int chatId) =>
        new(101, chatId, 11, "sender", 1, "sender", "message", null, DateTime.UtcNow);

    private async Task<Fixture> CreateFixture()
    {
        var context = CreateInMemoryContext();
        var features = new List<AccessFeature>
        {
            new() { Id = 1, Name = AccessFeatureStruct.UserLogin },
            new() { Id = 2, Name = AccessFeatureStruct.SendMessage }
        };
        var users = new[] { CreateTestUser(11), CreateTestUser(22), CreateTestUser(33) };
        foreach (var user in users) user.AccessFeatures = features;
        var chat = new ChatModel
        {
            Id = 701, Name = "Private group", IsGroup = true,
            CreatorId = users[0].Id, Creator = users[0], CoverId = 1,
            Cover = new ImageFile { Id = 1, ItemName = "cover", Url = "https://example.test/cover.png" },
            Members = new List<User> { users[0], users[1] }, Admins = new List<User>(), Messages = []
        };
        context.Users.AddRange(users);
        context.Chats.Add(chat);
        await context.SaveChangesAsync();

        var repository = new SequentialChatRepository(context);
        var transport = new RecordingTransport();
        var notifier = new ChatNotifier(transport.HubContext.Object, repository);
        var service = new ChatService(repository, Mock.Of<IImageFileService>(), Mock.Of<IMessageService>(),
            Mock.Of<IMessageReadService>(), Mock.Of<IUserService>(), notifier,
            Mock.Of<IUserFollowService>(), Mock.Of<IUserRepository>());
        return new Fixture(context, CreateUserManager(context), service, notifier, transport, chat,
            users[0], users[1], users[2], CreateClaimsPrincipal);
    }

    private sealed record Fixture(SonarContext Context, UserManager<User> UserManager, ChatService Service,
        ChatNotifier Notifier, RecordingTransport Transport, ChatModel Chat, User Owner, User Member, User Outsider,
        Func<User, System.Security.Claims.ClaimsPrincipal> PrincipalFactory) : IDisposable
    {
        public ChatHub CreateHub(User user, string connectionId)
        {
            var caller = new Mock<HubCallerContext>();
            caller.SetupGet(value => value.ConnectionId).Returns(connectionId);
            caller.SetupGet(value => value.User).Returns(PrincipalFactory(user));
            caller.SetupGet(value => value.Features).Returns(new FeatureCollection());
            return new ChatHub(new HttpContextAccessor(), UserManager, Service)
            {
                Context = caller.Object,
                Groups = Transport.Groups.Object,
                Clients = Transport.CallerClients.Object
            };
        }

        public void Dispose()
        {
            UserManager.Dispose();
            Context.Dispose();
        }
    }

    private sealed class RecordingTransport
    {
        private readonly Dictionary<string, HashSet<string>> subscriptions = new();
        public Mock<IGroupManager> Groups { get; } = new(MockBehavior.Strict);
        public Mock<IHubCallerClients> CallerClients { get; } = new(MockBehavior.Strict);
        public Mock<IHubContext<ChatHub>> HubContext { get; } = new(MockBehavior.Strict);
        public List<Delivery> Deliveries { get; } = [];

        public RecordingTransport()
        {
            Groups.Setup(value => value.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string connection, string group, CancellationToken _) =>
                {
                    if (!subscriptions.TryGetValue(group, out var connections))
                        subscriptions[group] = connections = [];
                    connections.Add(connection);
                    return Task.CompletedTask;
                });
            Groups.Setup(value => value.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string connection, string group, CancellationToken _) =>
                {
                    if (subscriptions.TryGetValue(group, out var connections)) connections.Remove(connection);
                    return Task.CompletedTask;
                });
            var clients = new Mock<IHubClients>(MockBehavior.Strict);
            clients.Setup(value => value.Groups(It.IsAny<IReadOnlyList<string>>())).Returns(CreateProxy);
            CallerClients.Setup(value => value.Groups(It.IsAny<IReadOnlyList<string>>())).Returns(CreateProxy);
            HubContext.SetupGet(value => value.Clients).Returns(clients.Object);
        }

        public string[] GroupsFor(string connection) => subscriptions.Where(group => group.Value.Contains(connection))
            .Select(group => group.Key).Order().ToArray();

        private IClientProxy CreateProxy(IReadOnlyList<string> groups)
        {
            var selectedGroups = groups.ToArray();
            var proxy = new Mock<IClientProxy>(MockBehavior.Strict);
            proxy.Setup(value => value.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                .Returns((string name, object?[] _, CancellationToken _) =>
                {
                    var connections = selectedGroups.Where(subscriptions.ContainsKey)
                        .SelectMany(group => subscriptions[group]).Distinct().Order().ToArray();
                    Deliveries.Add(new Delivery(name, connections));
                    return Task.CompletedTask;
                });
            return proxy.Object;
        }
    }

    private sealed record Delivery(string Name, string[] Connections);
}

public sealed class ChatDiagnosticAuthorizationTests
{
    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public async Task DiagnosticActionDoesNotExecuteOutsideDevelopment(string environmentName)
    {
        var notifier = new Mock<IChatNotifier>(MockBehavior.Strict);
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(value => value.EnvironmentName).Returns(environmentName);
        var controller = new ChatEventsTestController(notifier.Object, environment.Object);
        var action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        var context = new ActionExecutingContext(action, [], new Dictionary<string, object?>(), controller);
        bool executed = false;

        await ((IAsyncActionFilter)controller).OnActionExecutionAsync(context, () =>
        {
            executed = true;
            return Task.FromResult(new ActionExecutedContext(action, [], controller));
        });

        Assert.IsType<NotFoundResult>(context.Result);
        Assert.False(executed);
        notifier.VerifyNoOtherCalls();
    }

    [Fact]
    public void DevelopmentDiagnosticsStillRequireAuthentication()
    {
        var type = typeof(ChatEventsTestController);
        Assert.NotNull(type.GetCustomAttribute<AuthorizeAttribute>());
        Assert.Null(type.GetCustomAttribute<AllowAnonymousAttribute>());
        Assert.All(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
            method => Assert.Null(method.GetCustomAttribute<AllowAnonymousAttribute>()));

        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(value => value.EnvironmentName).Returns("Development");
        var controller = new ChatEventsTestController(Mock.Of<IChatNotifier>(), environment.Object);
        var action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        var context = new ActionExecutingContext(action, [], new Dictionary<string, object?>(), controller);
        controller.OnActionExecuting(context);
        Assert.Null(context.Result);
    }
}
