using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMP.BLL.Hubs;

namespace API.Installers
{
    public class HubsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(builder =>
            //   {
            //       builder.AllowAnyOrigin();
            //       builder.AllowAnyMethod();
            //   });
            //});
        }

    }
}
