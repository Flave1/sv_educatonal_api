using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Installers;
using System.IO;
using NLog;
using App.DomainObjects.Auth;
using App.Options;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssembly(Configuration);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.Use(async (ctx, next) => {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseRouting();
            app.UseStaticFiles();
            var swaggerOptions = new Options.SwaggerOptions();
            Configuration.GetSection(nameof(Options.SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });

            app.UseCors(MyAllowSpecificOrigins);

            app.UseSwaggerUI(option => {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandler("/errors/500");
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseMvc();
            CreateRolesAndAdminUser(serviceProvider).Wait();
        }







        private async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin" };

            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }

            var userSettings = new UserSettings();
            Configuration.GetSection(nameof(UserSettings)).Bind(userSettings);

            var adminPassword = userSettings.Password;
            var adminUser = new ApplicationUser()
            {
                Email = userSettings.Email,
                UserName = userSettings.UserName,
            };

            var user = await UserManager.FindByEmailAsync(userSettings.Email);
            if (user == null)
            {
                var created = await UserManager.CreateAsync(adminUser, adminPassword);
                if (created.Succeeded) { await UserManager.AddToRoleAsync(adminUser, "Admin"); }
            }
        }
    }
}
