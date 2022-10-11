using Contracts.Annoucements;
using SMP.Contracts.NotificationModels;
using System.Threading.Tasks;

namespace SMP.BLL.Services.NotififcationServices
{
    public interface INotificationService
    {
        void PushAnnouncementNotitfication(CreateAnnouncement request);
        Task CreateNotitfication(NotificationDTO request, bool byEmail = false, bool bySms = false);
    }
}