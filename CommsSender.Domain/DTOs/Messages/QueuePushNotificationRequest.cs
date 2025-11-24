using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.DTOs.Messages
{
    public class QueuePushNotificationRequest
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }
}
