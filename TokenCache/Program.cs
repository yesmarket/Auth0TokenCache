using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TokenCache
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //setup our DI
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .Configure<AuthSettings>(configuration.GetSection("authSettings"))
                .AddSingleton(resolver => resolver.GetRequiredService<IOptions<AuthSettings>>().Value)
                .AddSingleton<ITokenCache, TokenCache>()
                .BuildServiceProvider();

            var tokenCache = serviceProvider.GetService<ITokenCache>();

            var token1 = await tokenCache.FetchToken("test");
            Console.WriteLine($"First token (from Auth0) is:{Environment.NewLine}{token1}{Environment.NewLine}{Environment.NewLine}");

            var token2 = await tokenCache.FetchToken("test");
            Console.WriteLine($"Second token (from cache) is :{Environment.NewLine}{token2}");

            Console.ReadKey();
        }
    }
}
