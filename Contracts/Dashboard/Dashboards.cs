using System;

namespace SMP.Contracts.Dashboard
{
    public class GetDashboardCount
    { 
        public long TotaldStudent { get; set; }
        public long TotalClass { get; set; }
        public long TotalStaff { get; set; }
        public long TotalSubjects { get; set; }
        public long TotalUnusedPins { get; set; }
        public long TotalAssessments { get; set; }
    }

    public class GetTEacherDashboardCount
    {
        public long TotaldStudent { get; set; }
        public long TotalClass { get; set; }
        public long TotalSubjects { get; set; }
        public long TotalPins { get; set; }
        public long TotalUsedPins { get; set; }
        public long TotalUnusedPins { get; set; }
        public long TotalAssessments { get; set; }
    }

    public class GetStudentshDasboardCount
    {
        public long TotalSubjects { get; set; }
        public long TotalAssessments { get; set; }
        public long TotaldLessonNotes { get; set; }
        public long StudentNotes { get; set; }
    }

    public class Teacherclasses
    {
        public string SessionClass { get; set; }
        public Guid SessionClassId { get; set; }
        public int AssessmentCount { get; set; }
        public int StudentNoteCount { get; set; }
        public int StudentCounts { get; set; }
    }

    public class ApplicationSetupStatus
    {
        public string Setup { get; set;}
        public bool Cleared { get; set; }
        public decimal CompleteionStatus { get; set; }
        public string Message { get; set; }
    }
}
