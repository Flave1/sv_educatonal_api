using BLL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using SMP.Contracts.ResultModels;
using SMP.DAL.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ResultServices
{
    public interface IResultsService
    {
        Task<APIResponse<List<GetClasses>>> GetCurrentStaffClassesAsync();
        Task<APIResponse<List<GetClassSubjects>>> GetCurrentStaffClassSubjectsAsync(Guid sessionClassId);
        Task<APIResponse<List<GetClassScoreEntry>>> GetClassEntryAsync(Guid sessionClassId);
        Task CreateClassScoreEntryAsync(SessionClass sessionClass);
        Task CreateClassScoreSubjectEntriesAsync(Guid studentId);
    }
}