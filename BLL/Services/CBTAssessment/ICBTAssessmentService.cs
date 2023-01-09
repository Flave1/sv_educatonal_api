using BLL;
using BLL.Wrappers;
using SMP.Contracts.Assessment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.CBTAssessmentServices
{
    public interface ICBTAssessmentService
    {
        Task<APIResponse<PagedResponse<List<CBTExamination>>>> GetCBTAssessmentsAsync(string sessionClassId, int pageNumber);
    }
}