using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Annoucements;
using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AnnouncementsServices
{
    public interface IAnnouncementsService
    {
        Task<APIResponse<CreateAnnouncement>> CreateAnnouncementsAsync(CreateAnnouncement request);
        Task<APIResponse<PagedResponse<List<GetAnnouncements>>>> GetAnnouncementsAsync(PaginationFilter filter);
        Task<APIResponse<GetAnnouncements>> UpdateSeenAnnouncementAsync(UpdatSeenAnnouncement request);
        Task<APIResponse<UpdateAnnouncement>> UpdateAnnouncementsAsync(UpdateAnnouncement request);
        Task<APIResponse<bool>> DeleteAnnouncementsAsync(SingleDelete request);
    }
}