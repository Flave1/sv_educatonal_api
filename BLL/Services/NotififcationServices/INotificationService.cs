using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Annoucements;
using SMP.Contracts.NotificationModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NotififcationServices
{
    public interface INotificationService
    {
        Task CreateNotitficationAsync(NotificationDTO request, bool byEmail = false, bool bySms = false);
        Task<APIResponse<PagedResponse<List<GetNotificationDTO>>>> GetNotitficationAsync(PaginationFilter filter);
        Task UpdateNotification(Guid NotififcationId);
        Task<APIResponse<GetNotificationDTO>> GetSingleNotitficationAsync(Guid notificationId);
        Task<APIResponse<GetNotificationDTO>> GetMostRecentNotificationAsync();
        Task<APIResponse<PagedResponse<List<GetNotificationDTO>>>>  GetUnreadNotificationAsync(PaginationFilter filter);
        Task<APIResponse<int>> GetUnreadNotificationCountAsync();
    }
}