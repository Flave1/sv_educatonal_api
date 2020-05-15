using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentValidation.AspNetCore;
using System;
using System.Reflection;
using System.IO;
using App.Options;
using App.AuthHandler.Inplimentation;
using App.AuthHandler.Interface;
using App.MailHandler.Service;
using App.LogHandler.Service;
using App.Filters;

namespace App.Installers
{
    public class MvcInstaller : IInstaller
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);

            services.AddSingleton(jwtSettings);

            var tokenValidatorParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };

            services.AddSingleton(tokenValidatorParameters);

            services.AddScoped<IUriService, UriService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();
            })
                .AddFluentValidation(mvcConfuguration => mvcConfuguration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod(); ;
                });
            });

            services.AddHttpClient("GODP", client =>
            {
                client.BaseAddress = new Uri("http://localhost:56633/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryExample");
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidatorParameters;
            });

            services.AddSingleton<IUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUrl = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUrl);
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();



            services.AddTransient<IEmailService, EmailService>();

            services.AddSingleton<IEmailConfiguration>(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Flave Cloud",
                    Version = "V1",
                    Description = "An API to perform business automated operations",
                    TermsOfService = new Uri("http://www.godp.co.uk/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Flave Tech",
                        Email = "Flavetechs@gmail.com",
                        Url = new Uri("https://twitter.com/FavourE65881201"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GODP API LICX",
                        Url = new Uri("http://www.flave.co.uk/"),
                    },

                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //x.IncludeXmlComments(xmlPath);

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0] }
                };
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "FLAVE Cloud Authorization header using bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {new OpenApiSecurityScheme {Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    } }, new List<string>() }
                });
            });
        }
    }
}
