using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Authentication
{
   public class ApplicationRoles
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    }

    public class ApplicationRoleCommand
    {
        public string RoleId { get; set; }
        public string Name { get; set; } 

    }
}
