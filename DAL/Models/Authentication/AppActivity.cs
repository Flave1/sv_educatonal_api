﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Authentication
{
    public class AppActivity: CommonEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid Id { get; set; }
        public string Permission { get; set; }
        public string DisplayName { get; set; } 
        public bool IsActive { get; set; }
        public Guid ActivityParentId { get; set; }

        [ForeignKey("ActivityParentId")]
        public AppActivityParent Parent { get; set; }

    }
}
