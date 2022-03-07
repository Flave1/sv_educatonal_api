using Contracts.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.StudentServices
{
    public interface IStudentService
    {
        Task CreateStudenAsync(StudentContactCommand student);
        Task<List<GetStudentContacts>> GetAllStudensAsync();
    }
}
