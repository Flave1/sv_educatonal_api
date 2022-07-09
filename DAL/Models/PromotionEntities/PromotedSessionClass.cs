using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.PromotionEntities
{
    public class PromotedSessionClass : CommonEntity
    {
        [Key]
        public Guid PromotedClassId { get; set; }
        public Guid SessionClassId { get; set; }
        public Guid SessionId { get; set; }
        public bool IsPromoted { get; set; }
    }
}
