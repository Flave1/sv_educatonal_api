using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ClassEntities
{
    public class ClassLookup : CommonEntity
    { 
        [Key]
        public Guid ClassLookupId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
