using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces.Service.Utilities;
using StackExchange.Redis;

namespace Application.Services.Utilities
{
    public class VerificationService : IVerificationService
    {
        private readonly IDatabase _redisDb;
        private readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(1);
        private readonly TimeSpan _codeExpiration = TimeSpan.FromMinutes(10);

        public VerificationService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public Task<string> GenerateVerificationCode(int length = 6)
        {
            if (length <= 0) length = 6;

            var random = new Random();
            var code = "";
            for (int i = 0; i < length; i++)
            {
                code += random.Next(0, 10).ToString();
            }

            return Task.FromResult(code);
        }

        public Task<bool> ValidateVerificationCode(string code, string inputCode)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(inputCode))
                return Task.FromResult(false);

            return Task.FromResult(code == inputCode);
        }

        /// <summary>
        /// 
        /// </summary>

        public Task<string> GenerateVerificationTokenAsync(int userId)
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var payload = $"{userId}:{Convert.ToBase64String(randomBytes)}:{DateTime.UtcNow.Ticks}";
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

            string redisKey = GetTokenRedisKey(userId);
            _redisDb.StringSet(redisKey, token, _tokenExpiration);

            return Task.FromResult(token);
        }

        public async Task<bool> ValidateVerificationTokenAsync(string token, int userId)
        {
            string redisKey = GetTokenRedisKey(userId);
            var storedToken = await _redisDb.StringGetAsync(redisKey);

            if (storedToken.IsNullOrEmpty) return false;

            return storedToken == token;
        }

        private string GetTokenRedisKey(int userId) => $"VerificationToken:{userId}";
    }
}
