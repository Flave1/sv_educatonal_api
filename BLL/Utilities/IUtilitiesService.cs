using DAL.StudentInformation;
using SMP.DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Utilities
{
    public interface IUtilitiesService
    {
        string GetStudentRegNumberValue(string regNo, string clientId = null);
        Task<IDictionary<string, string>> GenerateStudentRegNo();
        Task<StudentContact> GetStudentContactByRegNo(string studentRegNoValue, string clientId = null);
    }
}
