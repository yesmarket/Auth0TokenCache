using System;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TokenCache
{
    public class TokenCache : ITokenCache
    {
        public const int Buffer = 600; // 10 minute buffer for cache expiration.

        private readonly AuthSettings _settings;
        private readonly IMemoryCache _cache;

        public TokenCache(
            IOptions<AuthSettings> settings)
        {
            _settings = settings?.Value ?? throw new ArgumentException(nameof(settings));
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<string> FetchToken(string key)
        {
            var token = await _cache.GetOrCreateAsync(key, async entry =>
            {
                Tenant tenant = null;
                if (!_settings?.Tenants.TryGetValue(key, out tenant) ?? true)
                {
                    throw new Exception("invalid tenant specified");
                }
                
                var client = new AuthenticationApiClient(new Uri(tenant.Domain));

                var credentials = new ClientCredentialsTokenRequest
                {
                    Audience = tenant.Audience,
                    ClientId = tenant.ClientId,
                    ClientSecret = tenant.ClientSecret
                };

                var accessTokenResponse = await client.GetTokenAsync(credentials);

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn - Buffer) ;

                return accessTokenResponse.AccessToken;
            });

            return token;
        }
    }
}
