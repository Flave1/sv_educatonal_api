using DAL.ClassEntities;
using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Timetable
{
    public class CreateExamTimeTable
    {
        public string ClassId { get; set; }
    }
    public class CreateExamTimeTableDay
    {
        public string Day { get; set; }
        public string ExamTimeTableId { get; set; }
    }
    public class UpdateExamTimeTableDay
    {
        public string Day { get; set; }
        public string ExamTimeTableId { get; set; }
        public string ExamTimeTableDayId { get; set; }
    }
    public class CreateExamTimeTableTime
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ExamTimeTableId { get; set; }
        public string ClassId { get; set; }
        public string ExamTimeTableTimeId { get; set; }
    }
    public class UpdateExamTimeTableTime
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ExamTimeTableTimeId { get; set; }
    }
    public class UpdateExamTimeTableTimeActivity
    {
        public string Activity { get; set; }
        public string ActivityId { get; set; }
    }
    public class GetExamTimeActivity
    {
        public string ClassName { get; set; }
        public string ExamTimeTableId { get; set; }
        public ExamTimetable Timetable { get; set; }

        public GetExamTimeActivity(ClassTimeTable db)
        {
            ClassName = db.Class.Name;
            ExamTimeTableId = db.ClassTimeTableId.ToString();
            Timetable = new ExamTimetable(db.Days, db.Times);
        }
    }
    public class ExamTimetable
    {
        public ExamDay[] days { get; set; } = Array.Empty<ExamDay>();
        public ExamTime[] times { get; set; } = Array.Empty<ExamTime>();
        public ExamTimetable(ICollection<ClassTimeTableDay> dayList, ICollection<ClassTimeTableTime> timeList)
        {
            if (dayList.Any())
            {
                days = dayList.Select(s => new ExamDay(s)).ToArray();
            }
            if (timeList.Any())
            {
                times = timeList.OrderBy(d => d.CreatedOn).Select(s => new ExamTime(s)).ToArray();
            }
        }

    }
    public class ExamPeriodActivities
    {
        public string activity { get; set; }
        public string activityId { get; set; }
        public string examTimeTableDayId { get; set; }
        public ExamPeriodActivities(ClassTimeTableTimeActivity db)
        {
            activity = db.Activity;
            examTimeTableDayId = db?.Day?.ClassTimeTableDayId.ToString();
            activityId = db.ClassTimeTableTimeActivityId.ToString();
        }
    }
    public class ExamDay
    {
        public string day { get; set; }
        public string examTimeTableDayId { get; set; }
        public ExamDay(ClassTimeTableDay db)
        {
            day = db.Day;
            examTimeTableDayId = db.ClassTimeTableDayId.ToString();
        }

    }
    public class ExamTime
    {
        public string examTimeTableTimeId { get; set; }
        public string period { get; set; }
        public ExamPeriodActivities[] periodActivities { get; set; } = Array.Empty<ExamPeriodActivities>();
        public ExamTime(ClassTimeTableTime time)
        {
            examTimeTableTimeId = time.ClassTimeTableTimeId.ToString();
            period = time.Start + " - " + time.End;
            periodActivities = time.Activities.Select(d => new ExamPeriodActivities(d)).ToArray();
        }
    }
    public class GetActiveTimetableClasses
    {
        public string LookupId { get; set; }
        public string Name { get; set; }
        public string GradeLevelId { get; set; }
        public bool IsActive { get; set; }
        public string GradeLevelName { get; set; }
        public string SessionClassId { get; set; }
        public GetActiveTimetableClasses(ClassLookup classLookup, SessionClass sessionClass)
        {
            LookupId = classLookup.ClassLookupId.ToString();
            Name = classLookup.Name;
            GradeLevelId = classLookup.GradeGroupId.ToString();
            IsActive = classLookup.IsActive;
            SessionClassId = sessionClass.SessionClassId.ToString();

        }
    }
}
