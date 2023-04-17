using System;

namespace DAL.SubjectModels
{
    public class SubjectDto
    {
        public Guid SubjectId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
