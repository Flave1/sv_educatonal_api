using BLL.AuthenticationServices;
using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Common;
using Contracts.Options;
using DAL;
using DAL.Authentication;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ParentServices;
using SMP.BLL.Services.SessionServices;
using SMP.BLL.Utilities;
using SMP.Contracts.Students;
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
        private readonly IUtilitiesService utilitiesService;
        private readonly ILoggerService loggerService;
        private readonly UserManager<AppUser> userManager;
        private readonly IFileUploadService upload;
        public readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;
        private readonly IParentService parentService;
        private readonly string smsClientId;
        private readonly ITermService termService;
        public StudentService(DataContext context, UserManager<AppUser> userManager, IFileUploadService upload,
            IHttpContextAccessor accessor, IPaginationService paginationService, IUserService userService, IParentService parentServices,
            IUtilitiesService utilitiesService, ILoggerService loggerService, ITermService termService)
        {
            this.context = context;
            this.userManager = userManager;
            this.upload = upload;
            this.accessor = accessor;
            this.paginationService = paginationService;
            this.userService = userService;
            this.utilitiesService = utilitiesService;
            this.loggerService = loggerService;
            this.parentService = parentServices;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
        }

        async Task<APIResponse<StudentContact>> IStudentService.CreateStudenAsync(StudentContactCommand student)
        {
            var res = new APIResponse<StudentContact>();
            try
            {
                var result = await utilitiesService.GenerateStudentRegNo();

                if (!result.Any())
                {
                    res.Message.FriendlyMessage = "School registration number not setup";
                    return res;
                }

                var userId = await userService.CreateStudentUserAccountAsync(student, result.Keys.First(), GetRegistrationFormat());
                
                var parentId = await parentService.SaveParentDetail(student.ParentOrGuardianEmail, student.ParentOrGuardianFirstName, student.ParentOrGuardianLastName, student.ParentOrGuardianRelationship, student.ParentOrGuardianPhone, Guid.Empty);
                var filePath = upload.UploadProfileImage(student.ProfileImage);
                var item = new StudentContact
                {
                    CityId = student.CityId,
                    CountryId = student.CountryId,
                    EmergencyPhone = student.EmergencyPhone,
                    HomeAddress = student.HomeAddress,
                    ParentId = parentId,
                    HomePhone = student.HomePhone,
                    StateId = student.StateId,
                    UserId = userId,
                    ZipCode = student.ZipCode,
                    RegistrationNumber = result.Keys.First(),
                    StudentContactId = Guid.NewGuid(),
                    Status = (int)StudentStatus.Active,
                    SessionClassId = Guid.Parse(student.SessionClassId),
                    EnrollmentStatus = (int)EnrollmentStatus.Enrolled,
                    LastName = student.LastName,
                    DOB = student.DOB,
                    FirstName = student.FirstName,
                    MiddleName = student.MiddleName,
                    Phone = student.Phone,
                    Photo = filePath,
                    ClientId = smsClientId,
                };
                context.StudentContact.Add(item);
                await context.SaveChangesAsync();

                await CreateStudentSessionClassHistoryAsync(item);
                res.Message.FriendlyMessage = Messages.Created;
                res.Result = null;
                res.IsSuccessful = true;
                return res;
            }
            catch (DuplicateNameException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            catch (ArgumentException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public string GetRegistrationFormat()
        {
            var regF = context.SchoolSettings.FirstOrDefault(d => d.ClientId == smsClientId);
            var reg = regF.SCHOOLSETTINGS_StudentRegNoFormat.Replace("%VALUE%", "");
            return reg.Replace(regF.SCHOOLSETTINGS_RegNoSeperator + "", "");
        }
        public async Task CreateStudentSessionClassHistoryAsync(StudentContact student)
        {
            var history  = new StudentSessionClassHistory();
            history.SessionClassId = student.SessionClassId;
            history.StudentContactId = student.StudentContactId;
            history.SessionTermId = termService.GetCurrentTerm().SessionTermId;
            await context.StudentSessionClassHistory.AddAsync(history);
            await context.SaveChangesAsync();
        }

        async Task<APIResponse<StudentContact>> IStudentService.UpdateStudenAsync(StudentContactCommand student)
        {
            var res = new APIResponse<StudentContact>();

            try
            {
                var studentInfor = await context.StudentContact.FirstOrDefaultAsync(a => a.StudentContactId == Guid.Parse(student.StudentAccountId) && a.ClientId == smsClientId);
                if (studentInfor == null)
                {
                    res.Message.FriendlyMessage = "Student Account not found";
                    return res;
                }
                var parentid = await parentService
                    .SaveParentDetail(
                    student.ParentOrGuardianEmail, 
                    student.ParentOrGuardianFirstName, 
                    student.ParentOrGuardianLastName, 
                    student.ParentOrGuardianRelationship, 
                    student.ParentOrGuardianPhone, 
                    studentInfor?.ParentId ?? Guid.Empty);

                await userService.UpdateStudentUserAccountAsync(student);

                var filePath = upload.UpdateProfileImage(student.ProfileImage, studentInfor.Photo);
                studentInfor.CityId = student.CityId;
                studentInfor.CountryId = student.CountryId;
                studentInfor.EmergencyPhone = student.EmergencyPhone;
                studentInfor.HomePhone = student.HomePhone;
                studentInfor.StateId = student.StateId;
                studentInfor.ZipCode = student.ZipCode;
                studentInfor.ParentId = parentid;

                studentInfor.LastName = student.LastName;
                studentInfor.DOB = student.DOB;
                studentInfor.FirstName = student.FirstName;
                studentInfor.MiddleName = student.MiddleName;
                studentInfor.Phone = student.Phone;
                studentInfor.Photo = filePath;
                studentInfor.ClientId = smsClientId;
                studentInfor.SessionClassId = Guid.Parse(student.SessionClassId);
                await context.SaveChangesAsync();

                res.Message.FriendlyMessage = "Updated student account successfully";
                res.Result = null;
                res.IsSuccessful = true;
                return res;
            }
            catch (DuplicateNameException ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = ex.Message;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }

        }

        async Task<APIResponse<UpdateProfileByStudentRequest>> IStudentService.UpdateProfileByStudentAsync(UpdateProfileByStudentRequest request)
        {
            var res = new APIResponse<UpdateProfileByStudentRequest>();

            try
            {
                var studentInfor = await context.StudentContact.FirstOrDefaultAsync(a => a.ClientId == smsClientId && a.StudentContactId == Guid.Parse(request.StudentContactId));
                if (studentInfor == null)
                {
                    res.Message.FriendlyMessage = "Student Account not found";
                    return res;
                }


                studentInfor.Hobbies = string.Join(',', request.Hobbies);
                studentInfor.BestSubjectIds = string.Join(',', request.BestSubjectIds);
                var filePath = upload.UpdateProfileImage(request.File, studentInfor.Photo);
                studentInfor.Photo = filePath;
                await context.SaveChangesAsync();

                res.Message.FriendlyMessage = Messages.Updated;
                res.Result = request;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }

        }

        async Task<APIResponse<PagedResponse<List<GetStudentContacts>>>> IStudentService.GetAllStudensAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetStudentContacts>>>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;

            var query = context.StudentContact.Where(x => x.ClientId == smsClientId && x.Deleted == false)
                .Include(x => x.User)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .OrderBy(s => s.FirstName);

             var totaltRecord = query.Count();
             var result = await paginationService.GetPagedResult(query, filter).Select(f => new GetStudentContacts(f, regNoFormat)).ToListAsync();
             res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<GetStudentContacts>> IStudentService.GetSingleStudentAsync(Guid studentContactId)
        {
            var res = new APIResponse<GetStudentContacts>();
            var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == smsClientId)?.SCHOOLSETTINGS_StudentRegNoFormat;
            var subjects = context.Subject.Where(d => d.IsActive && !d.Deleted && d.ClientId == smsClientId).ToList();
            var result = await context.StudentContact
                .Where(d => studentContactId == d.StudentContactId && d.ClientId == smsClientId)
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.User)
                .Include(x => x.SessionClass)
                .Include(x => x.Parent)
                .Include(q=> q.SessionClass).ThenInclude(s => s.Class)
                .Where(d => d.Deleted == false)
                .Select(f => new GetStudentContacts(f, regNoFormat, subjects)).FirstOrDefaultAsync();

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
                var act = context.StudentContact.FirstOrDefault(d => d.ClientId == smsClientId && d.StudentContactId == Guid.Parse(id));
                if (act != null)
                {
                    act.Deleted = true;
                    act.FirstName = "DELETED";
                    act.FirstName = "DELETED";
                    await context.SaveChangesAsync();
                }
            }

            res.Message.FriendlyMessage = Messages.DeletedSuccess;
            res.Result = true;
            res.IsSuccessful = true;
            return res;

        }

        async Task IStudentService.ChangeClassAsync(Guid studentId, Guid classId)
        {
            var std = await context.StudentContact.FirstOrDefaultAsync(wh => wh.StudentContactId == studentId && wh.ClientId == smsClientId);
            if (std != null)
            {
                std.SessionClassId = classId;
                std.EnrollmentStatus = (int)EnrollmentStatus.Enrolled;
                await CreateStudentSessionClassHistoryAsync(std);
            }
        }

        async Task<APIResponse<StudentContact>> IStudentService.UploadStudentsAsync()
        {
            var res = new APIResponse<StudentContact>();
            try
            {
                List<UploadStudentExcel> uploadedRecord = new List<UploadStudentExcel>();
                var files = accessor.HttpContext.Request.Form.Files;

                if (files.Count() == 0)
                {
                    res.Message.FriendlyMessage = $"No File selected";
                    return res;
                }
                var byteList = new List<byte[]>();
                foreach (var fileBit in files)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteList.Add(ms.ToArray());
                        }
                    }
                }

                if (byteList.Count() > 0)
                {
                    foreach (var item in byteList)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (MemoryStream stream = new MemoryStream(item))
                        using (ExcelPackage excelPackage = new ExcelPackage(stream))
                        {
                            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                            int totalRows = workSheet.Dimension.Rows;
                            int totalColumns = workSheet.Dimension.Columns;
                            if (totalColumns != 19)
                            {
                                res.Message.FriendlyMessage = $"Eighteen (19) Columns Expected";
                                return res;
                            }
                            for (int i = 2; i <= totalRows; i++)
                            {
                                uploadedRecord.Add(new UploadStudentExcel
                                {
                                    ExcelLineNumber = i,
                                    SessionClass = workSheet.Cells[i, 1].Value != null ? workSheet.Cells[i, 1].Value.ToString() : null,
                                    RegistrationNumber = workSheet.Cells[i, 2].Value != null ? workSheet.Cells[i, 2].Value.ToString() : null,
                                    FirstName = workSheet.Cells[i, 3].Value != null ? workSheet.Cells[i, 3].Value.ToString() : null,
                                    LastName = workSheet.Cells[i, 4].Value != null ? workSheet.Cells[i, 4].Value.ToString() : null,
                                    MiddleName = workSheet.Cells[i, 5].Value != null ? workSheet.Cells[i, 5].Value.ToString() : null,
                                    Phone = workSheet.Cells[i, 6].Value != null ? workSheet.Cells[i, 6].Value.ToString() : null,
                                    DOB = workSheet.Cells[i, 7].Value != null ? workSheet.Cells[i, 7].Value.ToString() : null,
                                    Email = workSheet.Cells[i, 8].Value != null ? workSheet.Cells[i, 8].Value.ToString() : null,
                                    HomePhone = workSheet.Cells[i, 9].Value != null ? workSheet.Cells[i, 9].Value.ToString() : null,
                                    EmergencyPhone = workSheet.Cells[i, 10].Value != null ? workSheet.Cells[i, 10].Value.ToString() : null,
                                    ParentOrGuardianFirstName = workSheet.Cells[i, 11].Value != null ? workSheet.Cells[i, 11].Value.ToString() : null,
                                    ParentOrGuardianRelationship = workSheet.Cells[i, 12].Value != null ? workSheet.Cells[i, 12].Value.ToString() : null,
                                    ParentOrGuardianPhone = workSheet.Cells[i, 13].Value != null ? workSheet.Cells[i, 13].Value.ToString() : null,
                                    ParentOrGuardianEmail = workSheet.Cells[i, 14].Value != null ? workSheet.Cells[i, 14].Value.ToString() : null,
                                    HomeAddress = workSheet.Cells[i, 15].Value != null ? workSheet.Cells[i, 15].Value.ToString() : null,
                                    CityId = workSheet.Cells[i, 16].Value != null ? workSheet.Cells[i, 16].Value.ToString() : null,
                                    StateId = workSheet.Cells[i, 17].Value != null ? workSheet.Cells[i, 17].Value.ToString() : null,
                                    CountryId = workSheet.Cells[i, 18].Value != null ? workSheet.Cells[i, 18].Value.ToString() : null,
                                    ZipCode = workSheet.Cells[i, 19].Value != null ? workSheet.Cells[i, 19].Value.ToString() : null
                                });
                            }
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        foreach (var item in uploadedRecord)
                        {
                            Guid parentid = Guid.Empty;
                            StudentContact std = null;
                            if (string.IsNullOrEmpty(item.SessionClass))
                            {
                                res.Message.FriendlyMessage = $"Class name cannot be empty detected on line {item.ExcelLineNumber}";
                                return res;
                            }
                            else
                            {
                                var clas = context.SessionClass.Where(x => x.ClientId == smsClientId).Include(x => x.Class).Include(x => x.Session)
                                    .FirstOrDefault(z => z.Class.Name.ToLower() == item.SessionClass.ToLower() && z.Deleted == false && z.Session.IsActive);
                                if (clas == null)
                                {
                                    res.Message.FriendlyMessage = $"Class name {item.SessionClass} cannot be found detected on line {item.ExcelLineNumber}";
                                    return res;
                                }
                                item.SessionClass = clas.SessionClassId.ToString();
                            }
                            if (string.IsNullOrEmpty(item.RegistrationNumber))
                            {
                                var regNo = await utilitiesService.GenerateStudentRegNo();
                                if (!regNo.Any())
                                {
                                    res.Message.FriendlyMessage = "School registration number not setup";
                                    return res;
                                }
                                item.RegistrationNumber = regNo.FirstOrDefault().Key;
                            }
                            else
                            {
                                var regNo = utilitiesService.GetStudentRegNumberValue(item.RegistrationNumber);
                                std = await utilitiesService.GetStudentContactByRegNo(regNo);
                                if (std == null)
                                {
                                    res.Message.FriendlyMessage = $"No Student found with registration number {item.RegistrationNumber} detected on line {item.ExcelLineNumber}";
                                    return res;
                                }
                                else
                                {
                                    item.RegistrationNumber = regNo;
                                }
                            }

                            if (string.IsNullOrEmpty(item.Email))
                            {
                                item.Email = item.RegistrationNumber + "@school.com";
                            }
                            if (string.IsNullOrEmpty(item.ParentOrGuardianEmail))
                            {
                                res.Message.FriendlyMessage = $"Parent or Guardian email must not be empty detected on line {item.ExcelLineNumber}";
                                return res;
                            }
                            else
                            {
                                parentid = await parentService.SaveParentDetail(item.ParentOrGuardianEmail, item.ParentOrGuardianFirstName, item.ParentOrGuardianLastName, item.ParentOrGuardianRelationship, item.ParentOrGuardianPhone, Guid.Empty);
                            }
                            try
                            {
                                if (std is null)
                                {
                                    var regNoFormat = context.SchoolSettings.FirstOrDefault(x=> x.ClientId == smsClientId).SCHOOLSETTINGS_StudentRegNoFormat;
                                    var rgNo = regNoFormat.Replace("%VALUE%", item.RegistrationNumber);
                                    var userId = await userService.CreateStudentUserAccountAsync(item, item.RegistrationNumber, rgNo);
                                    std = new StudentContact();
                                    std.CityId = std.CityId;
                                    std.CountryId = item.CountryId;
                                    std.EmergencyPhone = item.EmergencyPhone;
                                    std.HomeAddress = item.HomeAddress;
                                    std.ParentId = parentid;
                                    std.HomePhone = item.HomePhone;
                                    std.StateId = item.StateId;
                                    std.UserId = userId;
                                    std.ZipCode = item.ZipCode;
                                    std.RegistrationNumber = item.RegistrationNumber;
                                    std.StudentContactId = Guid.NewGuid();
                                    std.Status = (int)StudentStatus.Active;
                                    std.SessionClassId = Guid.Parse(item.SessionClass);
                                    std.EnrollmentStatus = (int)EnrollmentStatus.Enrolled;
                                    std.LastName = item.LastName;
                                    std.DOB = item.DOB;
                                    std.FirstName = item.FirstName;
                                    std.MiddleName = item.MiddleName;
                                    std.Phone = item.Phone;

                                    context.StudentContact.Add(std);
                                    await context.SaveChangesAsync();
                                    await CreateStudentSessionClassHistoryAsync(std);

                                }
                                else
                                {
                                    await userService.UpdateStudentUserAccountAsync(item, std.UserId, std.StudentContactId.ToString());
                                    std.CityId = item.CityId;
                                    std.CountryId = item.CountryId;
                                    std.EmergencyPhone = item.EmergencyPhone;
                                    std.HomeAddress = item.HomeAddress;
                                    std.ParentId = parentid;
                                    std.HomePhone = item.HomePhone;
                                    std.StateId = item.StateId;
                                    std.ZipCode = item.ZipCode;

                                    std.LastName = item.LastName;
                                    std.DOB = item.DOB;
                                    std.FirstName = item.FirstName;
                                    std.MiddleName = item.MiddleName;
                                    std.Phone = item.Phone;
                                    std.ClientId = smsClientId;
                                    std.SessionClassId = Guid.Parse(item.SessionClass);
                                    await context.SaveChangesAsync();
                                }
                                
                            }
                            catch (DuplicateNameException ex)
                            {
                                await transaction.RollbackAsync();
                                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                                res.Message.FriendlyMessage = ex.Message;
                                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                                return res;
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                                res.Message.FriendlyMessage = "Error Occurred trying to create student account!! Please contact system administrator";
                                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                                return res;
                            }
                        }
                        await transaction.CommitAsync();
                    }
                }
            }

            catch (ArgumentException ex)
            {
                res.Message.FriendlyMessage = $" {ex?.Message}";
                return res;
            }
            catch (Exception ex)
            {
                res.Message.FriendlyMessage = $" {ex?.Message}";
                return res;
            }

            res.Message.FriendlyMessage = Messages.Uploaded;
            res.Result = null;
            res.IsSuccessful = true;
            return res;
        }

        public async Task<APIResponse<GetStudentContactCbt>> GetSingleStudentByRegNoCbtAsync(string studentRegNo, string clientId)
        {
            var res = new APIResponse<GetStudentContactCbt>();
            try
            {
                string regNo = utilitiesService.GetStudentRegNumberValue(studentRegNo, clientId);
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == clientId).SCHOOLSETTINGS_StudentRegNoFormat;

                var result = await context.StudentContact
                    .Where(d => regNo == d.RegistrationNumber && d.Deleted != true && d.ClientId == clientId)
                    .Include(x => x.User)
                    .OrderByDescending(d => d.CreatedOn)
                    .OrderByDescending(s => s.RegistrationNumber)
                    .Select(f => new GetStudentContactCbt(f, regNoFormat)).FirstOrDefaultAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public async Task<APIResponse<PagedResponse<List<GetStudentContactCbt>>>> GetStudentBySessionClassCbtAsync(PaginationFilter filter, string sessionClassId, string clientId)
        {
            var res = new APIResponse<PagedResponse<List<GetStudentContactCbt>>>();
            try
            {
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == clientId).SCHOOLSETTINGS_StudentRegNoFormat;

                var query = context.StudentContact
                    .Where(d => d.SessionClassId == Guid.Parse(sessionClassId) && d.EnrollmentStatus == (int)EnrollmentStatus.Enrolled && d.Deleted != true && d.ClientId == clientId);

                var totaltRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter)
                    .Include(d => d.User)
                    .OrderByDescending(d => d.CreatedOn)
                    .OrderByDescending(s => s.RegistrationNumber)
                    .Select(f => new GetStudentContactCbt(f, regNoFormat)).ToListAsync();

                res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public async Task<APIResponse<List<GetStudentContactCbt>>> GetAllStudentBySessionClassCbtAsync(string sessionClassId, string clientId)
        {
            var res = new APIResponse<List<GetStudentContactCbt>>();
            try
            {
                var regNoFormat = context.SchoolSettings.FirstOrDefault(x => x.ClientId == clientId).SCHOOLSETTINGS_StudentRegNoFormat;
                var result = await context.StudentContact
                    .Where(d => d.SessionClassId == Guid.Parse(sessionClassId) && d.EnrollmentStatus == (int)EnrollmentStatus.Enrolled && d.Deleted != true && d.ClientId == clientId)
                    .Include(x => x.User)
                    .OrderByDescending(d => d.CreatedOn)
                    .OrderByDescending(s => s.RegistrationNumber)
                    .Select(f => new GetStudentContactCbt(f, regNoFormat)).ToListAsync();

                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
        }

        public async Task<APIResponse<byte[]>> DownloadStudentTemplate()
        {
            var res = new APIResponse<byte[]>();
            try
            {

                byte[] File = new byte[0];
                DataTable dt = new DataTable();
                dt.Columns.Add("Session Class");
                dt.Columns.Add("Registration Number");
                dt.Columns.Add("Firstname");
                dt.Columns.Add("Lastname");
                dt.Columns.Add("Middlename");
                dt.Columns.Add("Phone");
                dt.Columns.Add("DOB");
                dt.Columns.Add("Email");
                dt.Columns.Add("Home Phone");
                dt.Columns.Add("Emergency Phone");
                dt.Columns.Add("Parent Or Guardian Name");
                dt.Columns.Add("Parent Or Guardian Relationship");
                dt.Columns.Add("Parent Or Guardian Phone");
                dt.Columns.Add("Parent Or Guardian Email");
                dt.Columns.Add("Home Address");
                dt.Columns.Add("City Id");
                dt.Columns.Add("State Id");
                dt.Columns.Add("Country Id");
                dt.Columns.Add("Zip Code");

                for (var i = 1; i < 2 + 1; i++)
                {
                    var row = dt.NewRow();
                    row["Session Class"] = "change";
                    row["Registration Number"] = "change";
                    row["Firstname"] = "change";
                    row["Lastname"] = "change";
                    row["Middlename"] = "change";
                    row["Phone"] = "change";
                    row["DOB"] = "change";
                    row["Email"] = "change";
                    row["Home Phone"] = "change";
                    row["Emergency Phone"] = "change";
                    row["Parent Or Guardian Name"] = "change";
                    row["Parent Or Guardian Relationship"] = "change";
                    row["Parent Or Guardian Phone"] = "change";
                    row["Parent Or Guardian Email"] = "change";
                    row["Home Address"] = "change";
                    row["City Id"] = "change";
                    row["State Id"] = "change";
                    row["Country Id"] = "change";
                    row["Zip Code"] = "change";
                    dt.Rows.Add(row);
                }


                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet One");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    File = pck.GetAsByteArray();
                }


                res.IsSuccessful = true;
                res.Result = File;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        IQueryable<StudentContact> IStudentService.GetStudent(Guid studentContactId) => context.StudentContact.Where(x => x.StudentContactId == studentContactId);
       
    }


}
