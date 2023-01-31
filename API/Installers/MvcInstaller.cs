using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Microsoft.AspNetCore.Http.Features;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Contracts.Email;
using BLL.EmailServices;
using BLL.LoggerService;
using Contracts.Options;
using Microsoft.AspNetCore.Builder;
using SMP.Contracts.Options;
using SMP.BLL.Services.WebRequestServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using BLL.PaginationService.Services;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Utilities;
using SMP.BLL.Services.AdmissionServices;
using SMP.DAL.Models;

namespace API.Installers
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

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.Configure<EmailConfiguration>(
              configuration.GetSection("EmailConfiguration"));

            services.Configure<JwtSettings>(
         configuration.GetSection("JwtSettings"));

            services.Configure<RegNumber>(
                configuration.GetSection("RegNumber"));

                    services.Configure<SchoolSettings>(
               configuration.GetSection("SchoolSettings"));

                    services.Configure<FwsConfigSettings>(
               configuration.GetSection("FwsConfigSettings"));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddSingleton<IPaginationService, PaginationService>();
            services.AddScoped<IUtilitiesService, UtilitiesService>();
            services.AddScoped<IAdmissionSettingService, AdmissionSettingService>();
            services.AddScoped<ICandidateAdmissionService, CandidateAdmissionService>();
            services.AddScoped<IAdmissionService, AdmissionService>();
            services.AddSingleton<FwsClientInformation>();
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(mvcConfuguration => mvcConfuguration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 209715200;
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

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        StringValues authHeader = context.HttpContext.Request.Headers["Authorization"];
                        string accessToken = authHeader.ToString().Replace("Bearer ", "").Trim();

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&  (path.StartsWithSegments("/hubs")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpClient<IWebRequestService, WebRequestService>("FLAVECONSOLE", client =>
            {

                client.BaseAddress = new Uri(configuration.GetValue<string>("FwsConfigSettings:FwsBaseUrl"));
                client.Timeout = TimeSpan.FromSeconds(1000000);
                client.DefaultRequestHeaders.Clear();

            });




            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FLAVE TECHS",
                    Version = "V1",
                    Description = "An API to perform business automated operations",
                    TermsOfService = new Uri("http://www.godp.co.uk/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Emmanuel Favour",
                        Email = "favouremmanuel433@gmail.com",
                        //Url = new Uri("https://twitter.com/FavourE65881201"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "FLAVE API LICX",
                        //Url = new Uri("www.flavetechs.com"),
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
                    Description = "School Cloud Authorization header using bearer scheme",
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
