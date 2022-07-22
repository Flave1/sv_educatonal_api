using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.Contracts.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.DashboardServices
{
    public class DashboardService: IDashboardService
    {
        private readonly DataContext context;
        public DashboardService(DataContext context)
        {
            this.context = context;
        }

        async Task<APIResponse<GetDashboard>> IDashboardService.GetDashboardInfoAsync()
        {
            var res = new APIResponse<GetDashboard>();
            var totalEnrolledstudent  =  context.StudentContact.Where(x=>x.Deleted == false && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).Count();
            var totalClass  =  context.SessionClass.Where(x=>x.Deleted == false && x.Session.IsActive == true).Count();
            var totalSubject  = context.Subject.Where(x=>x.Deleted == false).Count();
            var totalPins  =   context.UploadedPin.Where(x=>x.Deleted == false).Count();
            var totalStaff =   context.Teacher.Where(x=>x.Deleted == false).Count();
            res.Result = new GetDashboard { TotalClass = totalClass, TotalEnrolledStudent = totalEnrolledstudent, TotalPins = totalPins, TotalStaff = totalStaff, TotalSubjects = totalSubject };
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    }
}
