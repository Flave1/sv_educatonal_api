using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Annoucements;
using SMP.Contracts.ParentModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.ParentServices
{
    public interface IParentService
    {
        Task<APIResponse<PagedResponse<List<MyWards>>>> GetMyWardsAsync(PaginationFilter filter);
        Task<Guid> SaveParentDetail(string email, string name, string relationship, string number, Guid id);
        Task<APIResponse<PagedResponse<List<GetAnnouncements>>>> GetAnnouncementsAsync(PaginationFilter filter);
        Task<APIResponse<GetAnnouncements>> GetAnnouncementDetailsAsync(string announcementId);
        Task<APIResponse<GetAnnouncements>> UpdateSeenAnnouncementAsync(UpdatSeenAnnouncement request);
        Task<APIResponse<PagedResponse<List<GetParents>>>> GetParentsAsync(PaginationFilter filter);
        Task<APIResponse<PagedResponse<List<GetParentWards>>>> GetParentWardsAsync(PaginationFilter filter, string parentId);
        Task<APIResponse<GetParents>> GetParentByIdAsync(string parentId);

    }
}