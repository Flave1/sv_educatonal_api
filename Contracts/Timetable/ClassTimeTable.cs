using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMP.Contracts.Timetable.Timetable;

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

    public class CreateClassTimeTableTime
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ClassTimeTableId { get; set; }
    }
    public class CreateClassTimeTableTimeActivity
    {
        public string Activity { get; set; }
        public string ClassTimeTableTimeId { get; set; }
        public string ClassTimeTableDayId { get; set; }
    }

    public class GetClassActivity
    {
        public string ClassName { get; set; }
        public string classTimeTableId { get; set; }
        public Timetable timetable { get; set; }

        public GetClassActivity(ClassTimeTable db)
        {
            var days = db.Days.ToList();
            var times = db.Times.ToList();
            ClassName = db.Class.Name;
            classTimeTableId = db.ClassTimeTableId.ToString();
            timetable = new Timetable
            {
                days = days.Select(id => new Day(id.ClassTimeTableDayId, db.ClassTimeTableId.ToString(), db.Days)).ToList(),
                times = times.Select(id => new Time(id.ClassTimeTableTimeId, db.Times)).ToList()

            };
       
        }

    }
    public class Time
    {
        public string classTimeTableTimeId { get; set; }
        public string period { get; set; }
        
    }

    public class Timetable
    {
        public List<Day> days { get; set; }
        public List<Time> times { get; set; }

        public class Day
        {
            public string day { get; set; }
            public string classTimeTableDayId { get; set; }

            public Day(Guid id, string classTimeTableId, ICollection<ClassTimeTableDay> timeTableDays)
            {
                var feedBack = timeTableDays.ToList().Where(s => s.ClassTimeTableId == Guid.Parse(classTimeTableId));
                day = feedBack.FirstOrDefault(x => x.ClassTimeTableDayId == id).Day;
                classTimeTableDayId = feedBack.FirstOrDefault(x => x.ClassTimeTableDayId == id).ClassTimeTableDayId.ToString();
            }
        }

        public class Time
        {
            public string classTimeTableTimeId { get; set; }
            public string period { get; set; }
            public Time(Guid id, ICollection<ClassTimeTableTime> classTimeTableTime)
            {
                
            }
        }
    }
}

