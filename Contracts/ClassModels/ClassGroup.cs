using DAL.StudentInformation;
using SMP.DAL.Models.ClassEntities;
using System.Collections.Generic;

namespace SMP.Contracts.ClassModels
{
    public class CreateClassGroup
    {
        public string GroupName { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string[] StudentContactIds { get; set; }
    }

    public class UpdateClassGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassSubjectId { get; set; }
        public bool IsActive { get; set; }
        public string[] StudentContactIds { get; set; }
    }

    public class GetClassGroupRequest
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string SessionClassId { get; set; }
        public string SessionClassName { get; set; }
        public string SessionClassSubjectId { get; set; }
        public string SubjectName { get; set; }
        public ClassGroupStudents[] ClassGroupStudents { get; set; }
        public GetClassGroupRequest(SessionClassGroup db, List<StudentContact> students)
        {
            GroupId = db.SessionClassGroupId.ToString();
            GroupName = db.GroupName.ToString();
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = db.SessionClassSubject.Subject.Name;
        }
    }

    public class ClassGroupStudents
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string StudentPhoto { get; set; }
    }
}
