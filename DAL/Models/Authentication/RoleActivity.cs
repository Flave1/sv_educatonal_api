using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Authentication
{
    public class RoleActivity: CommonEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string RoleId { get; set; }
        public Guid ActivityId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanImport { get; set; }
        public bool CanExport { get; set; }
        [ForeignKey("RoleId")]
        public UserRole UserRole { get; set; }
        [ForeignKey("ActivityId")]
        public AppActivity Activity { get; set; }
    }
}
