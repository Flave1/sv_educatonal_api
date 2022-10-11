namespace SMP.Contracts.NotificationModels
{
    public class SendNotification
    {
        public virtual string Message { get; set; }
    }

    public class NotificationDTO
    {
        public string NotificationEmailLink { get; set; } = string.Empty;
        public string NotificationPageLink { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Senders { get; set; } = string.Empty;
        public string Receivers { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public string Svg { get; set; } = string.Empty;
        public string NotificationSourceId { get; set; } = string.Empty;
    }
}
