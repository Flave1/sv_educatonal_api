using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.Contracts.Timetable
{
    public class CreateClassTimeTable
    {
        public string ClassId { get; set; }
    }
    public class CreateClassTimeTableDay
    {
        public string Day { get; set; }
        public string ClassTimeTableId { get; set; }
    }

    public class UpdateClassTimeTableDay
    {
        public string Day { get; set; }
        public string ClassTimeTableId { get; set; }
        public string ClassTimeTableDayId { get; set; }
    }

    public class CreateClassTimeTableTime
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ClassTimeTableId { get; set; }
        public string ClassId { get;set; }
        public string ClassTimeTableTimeId { get; set; }
    }
    public class UpdateClassTimeTableTime
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ClassTimeTableTimeId { get; set; }
    }
    public class UpdateClassTimeTableTimeActivity
    {
        public string Activity { get; set; }
        public string ActivityId { get; set; }
    }

    public class GetClassTimeActivity
    {
        public string ClassName { get; set; }
        public string ClassTimeTableId { get; set; }
        public Timetable Timetable { get; set; }

        public GetClassTimeActivity(ClassTimeTable db)
        {
            ClassName = db.Class.Name;
            ClassTimeTableId = db.ClassTimeTableId.ToString();
            Timetable = new Timetable(db.Days, db.Times);
        }
    }

    public class Timetable
    {
        public Day[] days { get; set; } = Array.Empty<Day>();
        public Time[] times { get; set; } = Array.Empty<Time>();
        public Timetable(ICollection<ClassTimeTableDay> dayList, ICollection<ClassTimeTableTime> timeList)
        {
            if (dayList.Any())
            {
                days = dayList.Select(s => new Day(s)).ToArray();
            }
            if (timeList.Any())
            {
                times = timeList.OrderBy(d => d.Start).Select(s => new Time(s)).ToArray();
            }
        }

    }

    public class PeriodActivities
    {
        public string activity { get; set; }
        public string activityId { get; set; }
        public string classTimeTableDayId { get; set; }
        public PeriodActivities(ClassTimeTableTimeActivity db)
        {
            activity = db.Activity;
            classTimeTableDayId = db?.Day?.ClassTimeTableDayId.ToString();
            activityId = db.ClassTimeTableTimeActivityId.ToString();
        }
    }

    public class Day
    {
        public string day { get; set; }
        public string classTimeTableDayId { get; set; }
        public Day(ClassTimeTableDay db)
        {
            day = db.Day;
            classTimeTableDayId = db.ClassTimeTableDayId.ToString();
        }

    }

    public class Time
    {
        public string classTimeTableTimeId { get; set; }
        public string period { get; set; }
        public PeriodActivities[] periodActivities { get; set; } = Array.Empty<PeriodActivities>();
        public Time(ClassTimeTableTime time)
        {
            classTimeTableTimeId = time.ClassTimeTableTimeId.ToString();
            period = time.Start + " - " + time.End;
            periodActivities = time.Activities.Select(d => new PeriodActivities(d)).ToArray();
        }
    }
}

