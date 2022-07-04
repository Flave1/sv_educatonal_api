﻿using DAL.Authentication;
using DAL.ClassEntities;
using DAL.SessionEntities;
using DAL.StudentInformation;
using DAL.SubjectModels;
using DAL.TeachersInfor;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SMP.DAL.Models.Attendance;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.EnrollmentEntities;
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.Register;
using SMP.DAL.Models.SessionEntities;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL
{
    public class DataContext : IdentityDbContext<AppUser>
    { 
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DataContext()
        {
        }

        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<ClassLookup> ClassLookUp { get; set; }
        public DbSet<StudentContact> StudentContact { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<StudentClassProgressions> StudentClassProgressions { get; set; }
        public DbSet<SessionClass> SessionClass { get; set; }
        public DbSet<SessionClassSubject> SessionClassSubject { get; set; }
        public DbSet<SessionTerm> SessionTerm { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<ActivityParent> ActivityParent { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<RoleActivity> RoleActivity { get; set; }
        public DbSet<StudentSessionClassHistory> StudentSessionClassHistory { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
        public DbSet<GradeGroup> GradeGroup { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<ClassGrade> ClassGrade { get; set; }
        public DbSet<StudentAttendance> StudentAttendance { get; set; }
        public DbSet<ClassRegister> ClassRegister { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Teacher>().HasOne<AppUser>(a => a.User).WithOne(d => d.Teacher).HasForeignKey<Teacher>(ad => ad.UserId);

            builder.Entity<StudentClassProgressions>()
                .HasOne<StudentContact>(s => s.Student)
                .WithMany(g => g.ClassProgressions)
                 .HasForeignKey(ad => ad.StudentId);
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var loggedInUserId = ""; // _accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "useradmin";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                { 
                    entry.Entity.Deleted = false; 
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.CreatedBy = loggedInUserId;
                }
                else
                {
                    entry.Entity.UpdatedOn = DateTime.Now;
                    entry.Entity.UpdatedBy = loggedInUserId;
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var loggedInUserId = ""; // _accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "useradmin";
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                { 
                    entry.Entity.Deleted = false; 
                    entry.Entity.CreatedOn = DateTime.Now;
                    entry.Entity.CreatedBy = loggedInUserId;
                }
                else
                {
                    entry.Entity.UpdatedOn = DateTime.Now;
                    entry.Entity.UpdatedBy = loggedInUserId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
