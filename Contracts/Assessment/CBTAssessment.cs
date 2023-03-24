using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.Assessment
{
    public class CBTExamination
    {
        public string ExaminationId { get; set; }
        public string ExamName_SubjectId { get; set; }
        public string ExamName_Subject { get; set; }
        public string CandidateCategoryId_ClassId { get; set; }
        public string CandidateCategory_Class { get; set; }
        public int ExamScore { get; set; }
        public string CandidateExaminationId { get; set; }
        public string Duration { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Instruction { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool UseAsExamScore { get; set; }
        public bool UseAsAssessmentScore { get; set; }
        public string AsExamScoreSessionAndTerm { get; set; }
        public string AsAssessmentScoreSessionAndTerm { get; set; }
        public bool UploadToSmpAsAssessment { get; set; }
        public bool UploadToSmpAsExam { get; set; }
        public int Status { get; set; }
        public int ExamintionType { get; set; }
        public int PassMark { get; set; }
        public string CandidateIds { get; set; }
        public string CreatedOn { get; set; }
        public string Percentage { get; set; }
        public int TotalCandidate { get; set; }

    }

    public class InCludeAssessmentRequest
    {
        public string sessionClassId { get; set; }
        public string subjectId { get; set; }
        public string studentRegNos { get; set; }
        public bool Include { get; set; }
        public string examId { get; set; }
    }

    public class SelectResult
    {
        public string CandidateName { get; set; }
        public string CandidateId { get; set; }
        public string ExaminationName { get; set; }
        public int TotalScore { get; set; }
        public string Status { get; set; }
    }
}
