using Sonar.Services.Authentication;
using Xunit;

namespace Sonar.Tests.Services.Authentication;

public class TwoFactorChallengeStoreTests
{
    private static TwoFactorChallengeStore.Identity Identity(int id = 17) =>
        new(id, "fixture-stamp", $"user{id}@example.invalid", "fixture-device");

    [Fact]
    public void CorrectCodeReturnsBoundIdentityOnlyOnce()
    {
        var store = new TwoFactorChallengeStore(TimeProvider.System);
        var issued = store.Issue(Identity())!;
        Assert.Equal(43, issued.Token.Length);
        Assert.Matches("^[0-9]{6}$", issued.Code);
        Assert.True(store.TryConsume(issued.Token, issued.Code, out var identity));
        Assert.Equal(Identity(), identity);
        Assert.False(store.TryConsume(issued.Token, issued.Code, out _));
    }

    [Fact]
    public void FiveFailedAttemptsInvalidateEvenTheCorrectCode()
    {
        var store = new TwoFactorChallengeStore(TimeProvider.System);
        var issued = store.Issue(Identity())!;
        var wrongCode = issued.Code == "000000" ? "000001" : "000000";
        for (var i = 0; i < TwoFactorChallengeStore.MaximumAttempts; i++)
            Assert.False(store.TryConsume(issued.Token, wrongCode, out _));
        Assert.False(store.TryConsume(issued.Token, issued.Code, out _));
    }

    [Fact]
    public void ChallengeExpiresAtItsDeadline()
    {
        var clock = new TestClock();
        var store = new TwoFactorChallengeStore(clock);
        var issued = store.Issue(Identity())!;
        Assert.Equal(clock.Now + TwoFactorChallengeStore.Lifetime, issued.ExpiresAt);
        clock.Now = issued.ExpiresAt;
        Assert.False(store.TryConsume(issued.Token, issued.Code, out _));
    }

    [Fact]
    public void NewPasswordVerifiedChallengeReplacesPreviousChallenge()
    {
        var store = new TwoFactorChallengeStore(TimeProvider.System);
        var first = store.Issue(Identity())!;
        var second = store.Issue(Identity())!;
        Assert.NotEqual(first.Token, second.Token);
        Assert.False(store.TryConsume(first.Token, first.Code, out _));
        Assert.True(store.TryConsume(second.Token, second.Code, out _));
    }

    [Fact]
    public void UnknownAndRevokedChallengesCannotBeConsumed()
    {
        var store = new TwoFactorChallengeStore(TimeProvider.System);
        var issued = store.Issue(Identity())!;
        Assert.False(store.TryConsume(new string('x', 43), issued.Code, out _));
        store.Revoke(issued.Token);
        Assert.False(store.TryConsume(issued.Token, issued.Code, out _));
    }

    [Fact]
    public async Task ConcurrentVerificationCanSucceedOnlyOnce()
    {
        var store = new TwoFactorChallengeStore(TimeProvider.System);
        var issued = store.Issue(Identity())!;
        var outcomes = await Task.WhenAll(Enumerable.Range(0, 20).Select(index =>
            Task.Run(() => store.TryConsume(issued.Token, issued.Code, out _))));
        Assert.Single(outcomes, success => success);
    }

    private sealed class TestClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }
}
