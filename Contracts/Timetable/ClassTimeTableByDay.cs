using DAL.ClassEntities;
using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Timetable
{
    public class GetClassTimeActivityByDay
    {
        public string Day { get; set; }
        public string ClassTimeTableDayId { get; set; }
        public TimetableObj Timetable { get; set; }

        public GetClassTimeActivityByDay(ClassTimeTable classTimeTable, ClassTimeTableDay day)
        {
            Day = day.Day;
            ClassTimeTableDayId = day.ClassTimeTableDayId.ToString();
            Timetable = new TimetableObj(classTimeTable.Times);
        }
    }

    public class TimetableObj
    {
        public TimeObj[] times { get; set; } = Array.Empty<TimeObj>();
        public TimetableObj(ICollection<ClassTimeTableTime> timeList)
        {
            if (timeList.Any())
            {
                times = timeList.OrderBy(d => d.Start).Select(s => new TimeObj(s)).ToArray();
            }
        }

    }

    public class PeriodActivitiesObj
    {
        public string activity { get; set; }
        public string classTimeTableDayId { get; set; }
        public PeriodActivitiesObj(ClassTimeTableTimeActivity db)
        {
            activity = db.Activity;
            classTimeTableDayId = db.Day.ClassTimeTableDayId.ToString();
        }
    }

    public class TimeObj
    {
        public string classId { get; set; }
        public string period { get; set; }
        public string classTimeTableId { get; set; }
        public PeriodActivitiesObj[] periodActivities { get; set; } = Array.Empty<PeriodActivitiesObj>();
        public TimeObj(ClassTimeTableTime time)
        {
            classId = time.TimeTable.ClassId.ToString();
            classTimeTableId = time.ClassTimeTableId.ToString();
            period = time.Start + " - " + time.End;
            periodActivities = time.Activities.Select(d => new PeriodActivitiesObj(d)).ToArray();
        }
    }
}
