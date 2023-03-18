using BLL;
using BLL.Wrappers;
using SMP.Contracts.Assessment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.CBTAssessmentServices
{
    public interface ICBTAssessmentService
    {
        Task<APIResponse<PagedResponse<List<CBTExamination>>>> GetCBTAssessmentsAsync(string sessionClassId,string subjectId, int pageNumber);
        Task<APIResponse<bool>> IncludeCBTAssessmentToScoreEntryAsAssessment(string sessionClassId, string subjectId, string studentRegNos, bool Include, string examId);
        Task<APIResponse<bool>> IncludeCBTAssessmentToScoreEntryAsExamination(string sessionClassId, string subjectId, string studentRegNos, bool Include, string examId);
    }
}