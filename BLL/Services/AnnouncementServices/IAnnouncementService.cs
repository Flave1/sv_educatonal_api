using BLL; 
using Contracts.Annoucements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AnnouncementsServices
{
    public interface IAnnouncementsService
    {
        Task<APIResponse<CreateAnnouncement>> CreateAnnouncementsAsync(CreateAnnouncement request);
        Task<APIResponse<List<GetAnnouncements>>> GetAnnouncementsAsync();
        Task<APIResponse<GetAnnouncements>> UpdateSeenAnnouncementAsync(UpdatSeenAnnouncement request);
        Task<APIResponse<UpdateAnnouncement>> UpdateAnnouncementsAsync(UpdateAnnouncement request);
    }
}