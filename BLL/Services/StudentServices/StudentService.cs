using BLL.AuthenticationServices;
using BLL.Constants;
using BLL.Utilities;
using Contracts.Options;
using DAL;
using DAL.StudentInformation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.StudentServices
{
    public class StudentService: IStudentService
    {
        private readonly DataContext context;
        private readonly IUserService userService;

        public StudentService(DataContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        async Task IStudentService.CreateStudenAsync(StudentContactCommand student)
        {

            var result = RegistrationNumber.GenerateForStudents();

           var userId =  await userService.CreateStudentUserAccountAsync(student, result.Keys.First(), result.Values.First());

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
                StudentContactId = Guid.NewGuid()
            };
            try
            {
                context.StudentContact.Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                DataContext _context = new DataContext();
                var user = await _context.Users.FindAsync(userId);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                throw new ArgumentException("Error Occurred trying to create student account!! Please contact system administrator");
            }
            finally { context.Dispose(); }
        }

        async Task<List<GetStudentContacts>> IStudentService.GetAllStudensAsync()
        {
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;
           return await context.StudentContact.Include(q => q.User).Where(d => d.Deleted == false && d.User.UserType == (int)UserTypes.Student).OrderByDescending(s => s.RegistrationNumber).Select(f => new GetStudentContacts(f, regNoFormat)).ToListAsync(); 
        }

     

    }
}
