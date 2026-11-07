using Infrastructure.Data;
using Infrastructure.Repository.Chat;

namespace Sonar.Tests.Services.Chat;

// EF InMemory is used only for sequential service/transport tests. Database
// concurrency and commit guarantees are tested separately against PostgreSQL.
internal sealed class SequentialChatRepository(SonarContext context) : ChatRepository(context)
{
    public override Task<TResult> ExecuteLockedAsync<TResult>(int chatId, Func<Task<TResult>> operation) => operation();
}
