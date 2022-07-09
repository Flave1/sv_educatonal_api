using API.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using BLL.Constants;
using DAL.Authentication;
using System.IO;
using NLog;
using BLL.Utilities;
using Contracts.Options;

namespace API
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
            RegistrationNumber.Initialize(Configuration);
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
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
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
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });

            app.UseCors(MyAllowSpecificOrigins); 

            app.UseSwaggerUI(option => {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
                option.InjectStylesheet("/css/site.css");
                option.InjectJavascript("/js/site.js");
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseExceptionHandler("/errors/500");
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseMvc();

            CreateRolesAndAdminUser(serviceProvider).Wait();
        }


        private async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            string[] roleNames = { DefaultRoles.SCHOOLADMIN, DefaultRoles.STUDENT, DefaultRoles.TEACHER };

            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new UserRole { Name = roleName });
                }
            }

            var userSettings = new UserSettings();
            Configuration.GetSection(nameof(UserSettings)).Bind(userSettings);

            var adminPassword = userSettings.Password;
            var adminUser = new AppUser()
            {
                Email = userSettings.Email,
                UserName = userSettings.UserName,
            };

            var user = await UserManager.FindByEmailAsync(userSettings.Email);
            if (user == null)
            {
                var created = await UserManager.CreateAsync(adminUser, adminPassword);
                if (created.Succeeded)
                {
                    var added = await UserManager.AddToRolesAsync(adminUser, roleNames);
                    if (!added.Succeeded)
                    {
                        Console.Write(added.Errors);
                    }
                }
            }
        }
    }
}
