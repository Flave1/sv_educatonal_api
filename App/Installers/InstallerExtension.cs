using App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace App.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
               typeof(IInstaller).IsAssignableFrom(x) && !x.IsAbstract).ToList();

            var instanceOfInstallers = installers.Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            instanceOfInstallers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
