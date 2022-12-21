using Contracts.Class;
using Contracts.Common;
using DAL.ClassEntities;
using SMP.Contracts.ClassModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public interface IClassGroupService
    {
        Task<APIResponse<CreateClassGroup>> CreateClassGroupAsync(CreateClassGroup request);
        Task<APIResponse<UpdateClassGroup>> UpdateClassGroupAsync(UpdateClassGroup request);
        Task<APIResponse<List<GetClassGroupRequest>>> GetAllClassGroupsAsync(Guid sessionClassId, Guid sessionClassSubjectId);
        Task<APIResponse<MultipleDelete>> DeleteClassGroupAsync(MultipleDelete GroupId);
        Task<APIResponse<GetClassGroupRequest>> GetSingleClassGroupsAsync(Guid groupId, Guid sessionClassId);
        Task<APIResponse<List<SessionClassSubjects>>> GetSessionClassSubjectsAsync(Guid sessionClassId);
        Task<APIResponse<List<SessionClassSubjects>>> GetSessionClassSubjectsCbtAsync(Guid sessionClassId);
    }
}
