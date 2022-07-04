﻿using API.Installers;
using BLL.AuthenticationServices;
using BLL.ClassServices;
using BLL.Services.SubjectServices;
using BLL.SessionServices;
using BLL.StudentServices;
using DAL;
using DAL.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMP.BLL.Services.AttendanceServices;
using SMP.BLL.Services.EnrollmentServices;
using SMP.BLL.Services.GradeServices;
using SMP.BLL.Services.PromorionServices;
using SMP.BLL.Services.TeacherServices;

namespace GODP.APIsContinuation.Installers
{
    public class DbInstaller : IInstaller
    { 
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<AppUser>(opt =>
            {
                opt.Password.RequiredLength = 5;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
            })
                .AddRoles<UserRole>()
                .AddEntityFrameworkStores<DataContext>();
              
            services.AddMvc();


            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IClassLookupService, ClassLookupService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IAttendanceService, AttendanceService>();

        }
    }
}
