using DAL.ClassEntities;
using System;

namespace SMP.Contracts.ClassModels
{
    public class ClassDto
    {
        public Guid SessionClassId { get; set; }
        public Guid SessionId { get; set; }
        public Guid ClassId { get; set; }
        public Guid? FormTeacherId { get; set; }
        public bool InSession { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPromoted { get; set; }
        public ClassDto(SessionClass sClass)
        {
            SessionClassId = sClass.SessionClassId;
            SessionId = sClass.SessionId;
            ClassId = sClass.ClassId;
            FormTeacherId = sClass.FormTeacherId;
            InSession = sClass.InSession;
            IsPublished = sClass.IsPublished;
            IsPromoted = sClass.IsPromoted;
        }
    }
}
