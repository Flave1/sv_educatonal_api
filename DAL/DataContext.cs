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
using SMP.DAL.Models.GradeEntities;
using SMP.DAL.Models.Register;
using SMP.DAL.Models.ResultModels;
using SMP.DAL.Models.SessionEntities;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Threading;
using System.Threading.Tasks;
using SMP.DAL.Models.PortalSettings;
using SMP.DAL.Models.Annoucement;
using SMP.DAL.Models.NoteEntities;
using SMP.DAL.Models.Timetable;
using SMP.DAL.Models.AssessmentEntities;
using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.Parents;
using SMP.DAL.Models.Admission;
using SMP.DAL.Models.Logger;
using SMP.DAL.Models.Authentication;

namespace DAL
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor accessor = null)
            : base(options)
        {
            this.accessor = accessor;
        }

        private readonly IHttpContextAccessor accessor;

        public DataContext() { }


        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<ClassLookup> ClassLookUp { get; set; }
        public DbSet<StudentContact> StudentContact { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<SessionClass> SessionClass { get; set; }
        public DbSet<SessionClassSubject> SessionClassSubject { get; set; }
        public DbSet<SessionTerm> SessionTerm { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<AppActivityParent> AppActivityParent { get; set; }
        public DbSet<AppActivity> AppActivity { get; set; }
        public DbSet<RoleActivity> RoleActivity { get; set; }
        public DbSet<StudentSessionClassHistory> StudentSessionClassHistory { get; set; }
        public DbSet<GradeGroup> GradeGroup { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<StudentAttendance> StudentAttendance { get; set; }
        public DbSet<ClassRegister> ClassRegister { get; set; }
        public DbSet<ScoreEntry> ScoreEntry { get; set; }
        public DbSet<SchoolSetting> SchoolSettings { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Announcements> Announcement { get; set; }
        public DbSet<ClassNote> ClassNote { get; set; }
        public DbSet<StudentNote> StudentNote { get; set; }
        public DbSet<TeacherClassNote> TeacherClassNote { get;set;}
        public DbSet<TeacherClassNoteComment> TeacherClassNoteComment { get; set; }
        public DbSet<StudentNoteComment> StudentNoteComment { get; set; }
        public DbSet<ClassTimeTable> ClassTimeTable { get; set; }
        public DbSet<ClassTimeTableDay> ClassTimeTableDay { get; set; }
        public DbSet<ClassTimeTableTimeActivity> ClassTimeTableTimeActivity { get; set; }
        public DbSet<ClassTimeTableTime> ClassTimeTableTime { get; set; }
        public DbSet<SessionClassGroup> SessionClassGroup { get; set; }
        public DbSet<ClassAssessment> ClassAssessment { get; set; }
        public DbSet<AssessmentScoreRecord> AssessmentScoreRecord { get; set; }
        public DbSet<HomeAssessment> HomeAssessment { get; set; }
        public DbSet<HomeAssessmentFeedBack> HomeAssessmentFeedBack { get; set; }
        public DbSet<Parents> Parents { get; set; }
        public DbSet<AdmissionSetting> AdmissionSettings { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<ScoreEntryHistory> ScoreEntryHistory { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<OTP> OTP { get; set; }
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
            base.OnModelCreating(builder);
        }

       
        public override int SaveChanges()
        {
            var loggedInUserId =  accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "";
            var smsClientId = accessor?.HttpContext?.Items["smsClientId"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(smsClientId))
            {
                throw new ArgumentNullException(nameof(smsClientId));
            }
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                { 
                    entry.Entity.Deleted = false; 
                    entry.Entity.CreatedOn = GetCurrentLocalDateTime();
                    entry.Entity.CreatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
                else
                {
                    entry.Entity.UpdatedOn = GetCurrentLocalDateTime();
                    entry.Entity.UpdatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
            }
            return base.SaveChanges();
        }
       
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var loggedInUserId = accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "";
            var smsClientId = accessor?.HttpContext?.Items["smsClientId"]?.ToString() ?? "";
            //if (string.IsNullOrEmpty(smsClientId))
            //{
            //    throw new ArgumentNullException(nameof(smsClientId));
            //}
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                if (entry.State == EntityState.Added)
                { 
                    entry.Entity.Deleted = false;   
                    entry.Entity.CreatedOn = GetCurrentLocalDateTime();
                    entry.Entity.CreatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
                else
                {
                    entry.Entity.UpdatedOn = GetCurrentLocalDateTime();
                    entry.Entity.UpdatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public Task<int> SaveChangesNoClientAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var loggedInUserId = accessor?.HttpContext?.User?.FindFirst(x => x?.Type == "userId")?.Value ?? "";
            var smsClientId = accessor?.HttpContext?.Items["smsClientId"]?.ToString() ?? "";
           
            foreach (var entry in ChangeTracker.Entries<Log>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Deleted = false;
                    entry.Entity.CreatedOn = GetCurrentLocalDateTime();
                    entry.Entity.CreatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
                else
                {
                    entry.Entity.UpdatedOn = GetCurrentLocalDateTime();
                    entry.Entity.UpdatedBy = loggedInUserId;
                    entry.Entity.ClientId = smsClientId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        private static DateTimeOffset GetServerDate()
        {
            return DateTimeOffset.Now.AddDays(1).Subtract(TimeSpan.FromHours(3));
        }
        private DateTime GetCurrentLocalDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "W. Central Africa Standard Time");
            return localTime;
        }
    }
}
