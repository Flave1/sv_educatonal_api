﻿using BLL;
using BLL.Filter;
using BLL.Wrappers;
using DAL.ClassEntities;
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
        Task<APIResponse<PagedResponse<GetClassScoreEntry>>> GetClassEntryAsync(Guid sessionClassId, Guid subjectId, PaginationFilter filter);
        Task CreateClassScoreEntryAsync(SessionClass sessionClass, Guid[] subjectIds);
        Task<APIResponse<ScoreEntry>> UpdateExamScore(UpdateScore request);
        Task<APIResponse<ScoreEntry>> UpdateAssessmentScore(UpdateScore request);
        Task<APIResponse<PagedResponse<PreviewClassScoreEntry>>> PreviewClassScoreEntry(Guid sessionClassId, Guid subjectId, PaginationFilter filter);
        Task<APIResponse<MasterList>> GetMasterListAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<PagedResponse<StudentResult>>> GetClassResultListAsync(Guid sessionClassId, Guid termId, PaginationFilter filter);
        Task<APIResponse<PublishResultRequest>> PublishResultAsync(PublishResultRequest request);
        Task<APIResponse<PagedResponse<GetClassScoreEntry>>> GetPreviousTermsClassSubjectScoreEntriesAsync(Guid sessionClassId, Guid subjectId, Guid sessionTermId, PaginationFilter filter);
        Task<APIResponse<ScoreEntry>> UpdatePreviousTermsExamScore(UpdateOtherSessionScore request);
        Task<APIResponse<ScoreEntry>> UpdatePreviousTermsAssessmentScore(UpdateOtherSessionScore request);
        Task<APIResponse<PagedResponse<PreviewClassScoreEntry>>> PreviewPreviousTermsClassScoreEntry(Guid sessionClassId, Guid subjectId, Guid sessionTermId, PaginationFilter filter);
        Task<APIResponse<CumulativeMasterList>> GetCumulativeMasterListAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<StudentCoreEntry>> GetSingleStudentScoreEntryAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
        Task<APIResponse<PreviewResult>> GetStudentResultForPreviewAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
        Task<StudentResultRecord> GetStudentResultOnPromotionAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
        //Task<StudentResultRecord> GetStudentResultOnPromotionAsync(Guid sessionClassId, Guid termId);
        Task UpdateStudentPrintStatusAsync(Guid classId, Guid studentId, bool isResultPrinted);
        Task<APIResponse<PrintResult>> GetStudentResultForPrintingAsync(Guid sessionClassId, Guid termId, Guid studentContactId);
        Task UpdateSessionClassArchiveAsync(Guid studentId, Guid termId, bool isPublished);
        Task<bool> AllResultPublishedAsync();
        Task<APIResponse<BatchPrintDetail>> GetStudentsForBachPrinting(Guid sessionClassId, Guid termId);
        Task<APIResponse<List<PrintResult>>> GetStudentResultForBatchPrintingAsync(Guid sessionClassId, Guid termId);
        Task<APIResponse<List<GetClassSubjects>>> GetCurrentStaffClassSubjects2Async(Guid classId, Guid sessionClassId);
        Task<APIResponse<List<PublishList>>> GetPublishedList();
        Task<APIResponse<List<GetClasses>>> GetFormTeacherClassesAsync();
    }
}