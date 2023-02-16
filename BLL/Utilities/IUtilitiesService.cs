using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Utilities
{
    public interface IUtilitiesService
    {
        string GetStudentRegNumberValue(string regNo);
        Task<IDictionary<string, string>> GenerateStudentRegNo();
    }
}
