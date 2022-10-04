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
}
