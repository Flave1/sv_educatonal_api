﻿using SMP.DAL.Models.PortalSettings;
using System;

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
        public string ReceiversEmail { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public string Svg { get; set; } = string.Empty;
        public string NotificationSourceId { get; set; } = string.Empty;
        public string Type { get; set; }
        public string ToGroup { get; set; }

        public NotificationDTO()
        {

        }
    }

    public class UpdateNotification
    {
        public Guid NotificationId { get; set; }
    }

    public class GetNotificationDTO
    {
        public string NotificationEmailLink { get; set; } = string.Empty;
        public string NotificationPageLink { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Senders { get; set; } = string.Empty;
        public string Receivers { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public string Svg { get; set; } = string.Empty;
        public string NotificationSourceId { get; set; } = string.Empty;
        public string DateCreated { get; set; }
        public Guid NotificationId { get; set; }
        public string ReadBy { get; set; }
        public GetNotificationDTO(Notification x)
        {
            Type = x.Type;
            Subject = x.Subject;
            Svg = x.Svg;
            NotificationEmailLink = x.NotificationEmailLink;
            NotificationPageLink = x.NotificationPageLink;
            NotificationSourceId = x.NotificationSourceId;
            DateCreated = x.CreatedOn.ToString("f");
            NotificationId = x.NotificationId;
            Content = x.Content;
            ReadBy = x.ReadBy;
        }
    }

}
