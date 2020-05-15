using AutoMapper;
using App.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using App.DomainObjects.Auth;

namespace App.Installers
{
    public class DbInstaller : IInstaller
    { 
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("DefaultConnection")));


            services.AddDefaultIdentity<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 5;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddAutoMapper(typeof(Startup)); 
            services.AddMediatR(typeof(Startup));
            services.AddMvc();  

            

        }
    }
}
