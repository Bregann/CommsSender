using CommsSender.Domain.DTOs.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.Interfaces.Api
{
    public interface IMessageService
    {
        Task ProcessPendingMessages();
        Task QueuePushNotification(QueuePushNotificationRequest message);
        Task QueueTelegramMessage(QueueTelegramMessageRequest message);
    }
}
