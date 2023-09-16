using BLL.Constants;
using DAL.StudentInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Utilities
{
    public interface IUtilitiesService
    {
        string GetStudentRegNumberValue(string regNo, string clientId = null);
        Task<IDictionary<string, string>> GenerateStudentRegNo();
        Task<StudentContact> GetStudentContactByRegNo(string studentRegNoValue, string clientId = null);
        string GetUserType(string userTpyes, UserTypes type);
        bool IsThisUser(UserTypes type, string userTpyes);
        string RemoveUserType(string userTypes, UserTypes type);
        string AddUserType(string userTypes, UserTypes type);
    }
}
