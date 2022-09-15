﻿using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.Contracts.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PromotionModels
{
    public class PreviousSessionClasses
    {
        public string SessionClassId { get; set; }
        public string SessionClassName { get; set; }
        public int TotalStudentsInClass { get; set; }
        public int TotalStudentsPassed { get; set; }
        public int TotalStudentsFailed { get; set; }
        public bool? IsPromoted { get; set; }
        public int PassedStudents { get; set; } = 0;
        public int FailedStudents { get; set; } = 0;
        public string PassedStudentIds { get; set; }
        public string FailedStudentIds { get; set; }
        public PreviousSessionClasses(SessionClass cl, Guid termId)
        {
            SessionClassId = cl.SessionClassId.ToString();
            SessionClassName = cl.Class.Name;
            TotalStudentsInClass = cl.Students.Count();
            IsPromoted = cl?.PromotedSessionClass?.IsPromoted ?? false;
            var studentRecords = cl.Students.Select(d => new StudentResultRecord(d, termId)).ToList();
            if (studentRecords.Any())
            {
                PassedStudentIds = string.Join(',', studentRecords.Where(d => d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                FailedStudentIds = string.Join(',', studentRecords.Where(d => !d.ShouldPromoteStudent).Select(s => s.StudentContactId));
                TotalStudentsPassed = studentRecords.Count(d => d.ShouldPromoteStudent);
                TotalStudentsFailed = studentRecords.Count(d => !d.ShouldPromoteStudent);
            }
            
        }

    }

    public class Promote
    {
        public string ClassToBePromoted { get; set; }
        public string ClassToPromoteTo { get; set; }

    }

    public class FetchPassedOrFailedStudents
    {
        public string StudentIds { get; set; }
    }

    public class GetStudents
    {
        public string StudentContactId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SessionClassId { get; set; }
        public string Status { get; set; } 
        public string RegistrationNumber { get; set; }
        public GetStudents(StudentContact student, string status, string regNoFormat)
        {
            RegistrationNumber = regNoFormat.Replace("%VALUE%", student.RegistrationNumber);
            StudentContactId = student.StudentContactId.ToString();
            StudentName = student.User.FirstName +" "+ student.User.LastName;
            ClassName = student.SessionClass.Class.Name;
            SessionClassId = student.SessionClassId.ToString();
            Status = status;
        }
    }

}
