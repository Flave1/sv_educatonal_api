using Contracts.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);
            if (!redisCacheSettings.Enabled)
            {
                return;
            }
            //services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheSettings.RedisConnectionString);
            //services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            //services.AddEasyCaching(options =>
            //{
            //    options.UseRedis(redisConfig =>
            //    {
            //        redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
            //        redisConfig.DBConfig.AllowAdmin = true;
            //    }, "redis1");
            //});
        }

    }
}
