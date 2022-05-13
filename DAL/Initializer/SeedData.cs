using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SMP.DAL.Initializer
{
    public static class DataSeedHandler
    {
        public static IWebHost SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();
                DALInitializers.SeedActivityParents(context);
                DALInitializers.SeedActivities(context);
            }
            return host;
        }
    }
}
