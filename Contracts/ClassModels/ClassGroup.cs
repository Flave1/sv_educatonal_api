using DAL.StudentInformation;
using SMP.DAL.Models.ClassEntities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public int NumberOfStudentsInGroup { get; set; }
        public int NumberOfStudentInClass { get; set; }
        public int NumberOfStudentNotInGroup { get; set; }
        public ClassGroupStudents[] ClassGroupStudents { get; set; }
        public GetClassGroupRequest(SessionClassGroup db, List<StudentContact> students)
        {
            GroupId = db.SessionClassGroupId.ToString();
            GroupName = db.GroupName.ToString();
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = db.SessionClassSubject.Subject.Name;
            NumberOfStudentsInGroup = !string.IsNullOrEmpty(db.ListOfStudentContactIds) ? db.ListOfStudentContactIds.Split(',').Length : 0;
            NumberOfStudentInClass = students.Count();
            NumberOfStudentNotInGroup = Convert.ToInt32((NumberOfStudentsInGroup - students.Count()).ToString().TrimStart('-'));
            if (!string.IsNullOrEmpty(db.ListOfStudentContactIds))
            {
                if (students.Any())
                {
                    ClassGroupStudents = db.ListOfStudentContactIds.Split(",").ToList().Select(s => new ClassGroupStudents(s, students)).ToArray();
                }
                
            }
        }
        public GetClassGroupRequest(SessionClassGroup db, int totalStudents)
        {
            GroupId = db.SessionClassGroupId.ToString();
            GroupName = db.GroupName.ToString();
            SessionClassId = db.SessionClassId.ToString();
            SessionClassName = db.SessionClass.Class.Name;
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = db.SessionClassSubject.Subject.Name;
            NumberOfStudentsInGroup = !string.IsNullOrEmpty(db.ListOfStudentContactIds) ? db.ListOfStudentContactIds.Split(',').Length : 0;
            NumberOfStudentInClass = totalStudents;
            NumberOfStudentNotInGroup = Convert.ToInt32((NumberOfStudentsInGroup - totalStudents).ToString().TrimStart('-'));
        }
    }

    public class ClassGroupStudents
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string StudentPhoto { get; set; }
        public ClassGroupStudents(string id, List<StudentContact> students)
        {
            var std = students.FirstOrDefault(d => d.StudentContactId == Guid.Parse(id));
            StudentContactId = id;
            StudentName = std?.FirstName + "  " +  std?.FirstName;
            StudentPhoto = std?.Photo;
        }
    }

    public class SessionClassSubjects
    {
        public string SessionClassSubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Subjectid { get; set; }
        public SessionClassSubjects(SessionClassSubject db)
        {
            SessionClassSubjectId = db.SessionClassSubjectId.ToString();
            SubjectName = db.Subject.Name;
            Subjectid = db.SubjectId.ToString();
        }
    }
}
