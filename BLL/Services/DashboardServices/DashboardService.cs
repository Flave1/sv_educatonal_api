using BLL;
using BLL.Constants;
using DAL;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor accessor;

        public DashboardService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<GetDashboard>> IDashboardService.GetDashboardCountAsync()
        {
            var res = new APIResponse<GetDashboard>();
            var userid = accessor.HttpContext.User.FindFirst(e => e.Type == "userId")?.Value;

            var totalEnrolledstudent = context.StudentContact.Where(x => x.Deleted == false && x.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).Count();
            var totalClass = context.SessionClass.Where(x => x.Deleted == false && x.Session.IsActive == true).Count();
            var totalSubject = context.Subject.Where(x => x.Deleted == false && x.IsActive == true).Count();
            var totalPins = context.UploadedPin.Where(x => x.Deleted == false).Count();
            var totalStaff = context.Teacher.Where(x => x.Deleted == false).Count();
            if (!string.IsNullOrEmpty(userid))
            { 

                if(accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN))
                {
                    res.Result = new GetDashboard { TotalClass = totalClass, TotalEnrolledStudent = totalEnrolledstudent, TotalPins = totalPins, TotalStaff = totalStaff, TotalSubjects = totalSubject };
                }
                else
                {
                    res.Result = new GetDashboard { TotalClass = totalClass, TotalEnrolledStudent = totalEnrolledstudent, TotalPins = totalPins, TotalStaff = totalStaff, TotalSubjects = totalSubject };
                }
            }
            else
            { 
                res.Message.FriendlyMessage = "Invalid User";
                return res;
            }
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }
    }
}
