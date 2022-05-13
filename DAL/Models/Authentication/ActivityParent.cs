using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Authentication
{
    public class ActivityParent: CommonEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public virtual ICollection<Activity>  Activities { get; set; }
    }
}
