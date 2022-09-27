using Contracts.Annoucements;

namespace SMP.BLL.Services.NotififcationServices
{
    public interface INotificationService
    {
        void PushAnnouncementNotitfication(CreateAnnouncement request);
    }
}