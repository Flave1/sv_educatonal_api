using BLL; 
using Contracts.Annoucements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AnnouncementsServices
{
    public interface IAnnouncementsService
    {
        Task<APIResponse<AnnouncementsContract>> CreateAnnouncementsAsync(AnnouncementsContract request);
        Task<APIResponse<List<GetAnnouncementsContract>>> GetAnnouncementsAsync();
        Task<APIResponse<AnnouncementsContract>> UpdateAnnouncementsAsync(AnnouncementsContract request);
    }
}