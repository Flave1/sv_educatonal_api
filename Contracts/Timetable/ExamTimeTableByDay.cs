﻿using SMP.DAL.Models.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Timetable
{
    public class GetExamDays
    {
        public string ExamDay { get; set; }
        public string ExamTimeTableDayId { get; set; }

        public GetExamDays(ClassTimeTableDay day)
        {
            ExamDay = day.Day;
            ExamTimeTableDayId = day.ClassTimeTableDayId.ToString();
        }
    }
    public class GetExamTimeActivityByDay
    {
        public string Day { get; set; }
        public string ExamTimeTableDayId { get; set; }
        public ExamClassObj[] classes { get; set; } = Array.Empty<ExamClassObj>();
        public ExamTimeObj[] times { get; set; } = Array.Empty<ExamTimeObj>();

        public GetExamTimeActivityByDay(ClassTimeTable examTimeTable, List<ClassTimeTable> tableList)
        {
            Day = examTimeTable.Days.FirstOrDefault().Day;
            ExamTimeTableDayId = examTimeTable.Days.FirstOrDefault().ClassTimeTableDayId.ToString();
            if (tableList.Any())
            {
                classes = tableList.Select(s => new ExamClassObj(s)).ToArray();
            }
            if (examTimeTable.Times.Any())
            {
                times = examTimeTable.Times.OrderBy(d => d.Start).Select(s => new ExamTimeObj(s)).ToArray();
            }
        }
    }

    public class ExamPeriodActivitiesObj
    {
        public string activity { get; set; }
        public string classTimeTableDayId { get; set; }
        public ExamPeriodActivitiesObj(ClassTimeTableTimeActivity db)
        {
            activity = db.Activity;
            classTimeTableDayId = db.Day.ClassTimeTableDayId.ToString();
        }
    }

    public class ExamTimeObj
    {
        public string classId { get; set; }
        public string period { get; set; }
        public string examTimeTableId { get; set; }
        public ExamPeriodActivitiesObj[] periodActivities { get; set; } = Array.Empty<ExamPeriodActivitiesObj>();
        public ExamTimeObj(ClassTimeTableTime time)
        {
            classId = time.TimeTable.ClassId.ToString();
            examTimeTableId = time.ClassTimeTableId.ToString();
            period = time.Start + " - " + time.End;
            periodActivities = time.Activities.Select(d => new ExamPeriodActivitiesObj(d)).ToArray();
        }
    }

    public class ExamClassObj
    {
        public string classId { get; set; }
        public string className { get; set; }
        public string ExamTimeTableId { get; set; }
        public ExamClassObj(ClassTimeTable classs)
        {
            classId = classs.ClassId.ToString();
            className = classs.Class.Name.ToString();
            ExamTimeTableId = classs.ClassTimeTableId.ToString();
        }
    }
}
