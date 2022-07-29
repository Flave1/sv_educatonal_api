using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Constants
{
    public class UserConstants
    {
        public const string PASSWORD = "School@1";
    }



    public enum UserTypes
    {
        Admin = -1,
        Teacher = 1,
        Student = 0,
        Parent = 2
    }
}
