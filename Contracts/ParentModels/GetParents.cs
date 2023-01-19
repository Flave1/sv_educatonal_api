using SMP.DAL.Models.Parents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.ParentModels
{
    public class GetParents
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Relationship { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public GetParents(Parents parent)
        {
            ParentId = parent.Parentid.ToString();
            Name = parent.Name;
            Number = parent.Number;
            Relationship = parent.Relationship;
            Email = parent.Email;
            UserId = parent.UserId;
        }
    }
}
