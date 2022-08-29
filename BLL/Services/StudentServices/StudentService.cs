using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Filter;
using BLL.Helpers;
using BLL.PaginationService.Services;
using BLL.Utilities;
using Contracts.Common;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.EnrollmentServices;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.ResultServices;
using SMP.DAL.Models.EnrollmentEntities;
using SMP.DAL.Models.StudentImformation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.StudentServices
{
    public class StudentService : IStudentService
    {
        private readonly DataContext context;
        private readonly IUserService userService;
        private readonly UserManager<AppUser> userManager;
        private readonly IResultsService resultsService; 
        private readonly IFileUploadService upload;
        private readonly IUriService uriService;

        public StudentService(DataContext context, IUserService userService, UserManager<AppUser> userManager, IResultsService resultsService, IFileUploadService upload, IUriService uriService)
        {
            this.context = context;
            this.userService = userService;
            this.userManager = userManager;
            this.resultsService = resultsService; 
            this.upload = upload;
            this.uriService = uriService;
        }

        async Task<APIResponse<StudentContact>> IStudentService.CreateStudenAsync(StudentContactCommand student)
        {
            var res = new APIResponse<StudentContact>();
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                
                try
                { 
                    var result = RegistrationNumber.GenerateForStudents();

                    var userId = await userService.CreateStudentUserAccountAsync(student, result.Keys.First(), result.Values.First());
                     
                    var item = new StudentContact
                    {
                        CityId = student.CityId,
                        CountryId = student.CountryId,
                        EmergencyPhone = student.EmergencyPhone,
                        HomeAddress = student.HomeAddress,
                        ParentOrGuardianEmail = student.ParentOrGuardianEmail,
                        ParentOrGuardianName = student.ParentOrGuardianName,
                        ParentOrGuardianPhone = student.ParentOrGuardianPhone,
                        ParentOrGuardianRelationship = student.ParentOrGuardianRelationship,
                        HomePhone = student.HomePhone,
                        StateId = student.StateId,
                        UserId = userId, 
                        ZipCode = student.ZipCode,
                        RegistrationNumber = result.Keys.First(),
                        StudentContactId = Guid.NewGuid(),
                        Status = (int)StudentStatus.Active,
                        SessionClassId = Guid.Parse(student.SessionClassId),
                        EnrollmentStatus = (int)EnrollmentStatus.Enrolled
                        
                    };
                    context.StudentContact.Add(item);
                    await context.SaveChangesAsync();

                    await CreateStudentSessionClassHistoryAsync(item);
                    
                    await EnrollOnCreateStudentAsync(item);


                    await transaction.CommitAsync();
                    res.Message.FriendlyMessage = Messages.Created;
                    res.Result = null;
                    res.IsSuccessful = true;
                    return res;
                }
                catch (DuplicateNameException ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = ex.Message;
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }

               
                finally { await transaction.DisposeAsync(); }
            }
        }

        async Task CreateStudentSessionClassHistoryAsync(StudentContact student)
        {
            var history  = new StudentSessionClassHistory();
            history.SessionClassId = student.SessionClassId;
            history.StudentContactId = student.StudentContactId;
            history.SessionTermId = context.SessionTerm.FirstOrDefault(s => s.IsActive)?.SessionTermId;
            await context.StudentSessionClassHistory.AddAsync(history);
            await context.SaveChangesAsync();
        }

        async Task<APIResponse<StudentContact>> IStudentService.UpdateStudenAsync(StudentContactCommand student)
        {
            var res = new APIResponse<StudentContact>();

            using (var transaction = await context.Database.BeginTransactionAsync())
            {

                try
                {
                    var studentInfor = await context.StudentContact.FirstOrDefaultAsync(a => a.StudentContactId == Guid.Parse(student.StudentAccountId));
                    if (studentInfor == null)
                    {
                        res.Message.FriendlyMessage = "Student Account not found";
                        return res;
                    }

                    await userService.UpdateStudentUserAccountAsync(student);

                    studentInfor.CityId = student.CityId;
                    studentInfor.CountryId = student.CountryId;
                    studentInfor.EmergencyPhone = student.EmergencyPhone;
                    studentInfor.HomeAddress = student.HomeAddress;
                    studentInfor.ParentOrGuardianEmail = student.ParentOrGuardianEmail;
                    studentInfor.ParentOrGuardianName = student.ParentOrGuardianName;
                    studentInfor.ParentOrGuardianPhone = student.ParentOrGuardianPhone;
                    studentInfor.ParentOrGuardianRelationship = student.ParentOrGuardianRelationship;
                    studentInfor.HomePhone = student.HomePhone;
                    studentInfor.StateId = student.StateId;
                    studentInfor.ZipCode = student.ZipCode;
                    studentInfor.SessionClassId = Guid.Parse(student.SessionClassId);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    res.Message.FriendlyMessage = "Updated student account successfully";
                    res.Result = null;
                    res.IsSuccessful = true;
                    return res;
                }
                catch (DuplicateNameException ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = ex.Message;
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                    res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                    return res;
                }


                finally { await transaction.DisposeAsync(); }
            }
        }

        async Task<APIResponse<UpdateProfileByStudentRequest>> IStudentService.UpdateProfileByStudentAsync(UpdateProfileByStudentRequest request)
        {
            var res = new APIResponse<UpdateProfileByStudentRequest>();

            try
            {
                var studentInfor = await context.StudentContact.FirstOrDefaultAsync(a => a.StudentContactId == Guid.Parse(request.StudentContactId));
                if (studentInfor == null)
                {
                    res.Message.FriendlyMessage = "Student Account not found";
                    return res;
                }

                await userService.UpdateStudentUserProfileImageAsync(request.File, studentInfor.UserId);

                studentInfor.Hobbies = string.Join(',', request.Hobbies);
                studentInfor.BestSubjectIds = string.Join(',', request.BestSubjectIds);
                await context.SaveChangesAsync();

                res.Message.FriendlyMessage = Messages.Updated;
                res.Result = request;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }

        }

        async Task<APIResponse<List<GetStudentContacts>>> IStudentService.GetAllStudensAsync(PaginationFilter filter)
        {
            var res = new APIResponse<List<GetStudentContacts>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
            //PaginationFilter filter = new PaginationFilter(filter.PageNumber, filter.PageSize);
             
            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                .Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Student)
                .Select(f => new GetStudentContacts(f, regNoFormat)).ToListAsync();
            var totalRecords = await context.StudentContact.CountAsync(); 

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<GetStudentContacts>> IStudentService.GetSingleStudentAsync(Guid studentContactId)
        {
            var res = new APIResponse<GetStudentContacts>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.User)
                .Include(x => x.SessionClass)
                .Include(q=> q.SessionClass).ThenInclude(s => s.Class)
                .Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Student && studentContactId == d.StudentContactId)
                .Select(f => new GetStudentContacts(f, regNoFormat, context.Subject.Where(d => d.IsActive && !d.Deleted).ToList())).FirstOrDefaultAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<bool>> IStudentService.DeleteStudentAsync(MultipleDelete request)
        {
            var res = new APIResponse<bool>();

            foreach (var id in request.Items)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var act = context.StudentContact.FirstOrDefault(d => d.UserId == user.Id);
                if (act != null)
                {
                    act.Deleted = true;
                    user.Deleted = true;
                    user.Active = false;
                    user.Email = "DELETE" + user.Email;
                    user.UserName = "DELETE" + user.UserName;
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        res.Message.FriendlyMessage = result.Errors.FirstOrDefault().Description;
                        return res;
                    }
                }
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.Result = true;
            res.IsSuccessful = true;
            return res;

        }

        async Task IStudentService.ChangeClassAsync(Guid studentId, Guid classId)
        {
            try
            {
                var std = await context.StudentContact.FirstOrDefaultAsync(wh => wh.StudentContactId == studentId);
                if (std != null)
                {
                    std.SessionClassId = classId;
                    std.EnrollmentStatus = (int)EnrollmentStatus.Enrolled;
                    await CreateStudentSessionClassHistoryAsync(std);
                }
            }
            catch (Exception)
            { 
                throw;
            }
        }

        async Task EnrollOnCreateStudentAsync(StudentContact std)
        {
            var enrollment = new Enrollment();
            enrollment.StudentContactId = std.StudentContactId;
            enrollment.SessionClassId = std.SessionClassId;
            enrollment.Status = (int)EnrollmentStatus.Enrolled;
            await context.AddAsync(enrollment);
            await context.SaveChangesAsync();

          
        }
    }


}
