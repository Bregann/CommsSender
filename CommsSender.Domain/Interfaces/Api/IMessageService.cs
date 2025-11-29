using CommsSender.Domain.DTOs.Messages;

namespace CommsSender.Domain.Interfaces.Api
{
    public interface IMessageService
    {
        Task ProcessPendingMessages();
        Task QueuePushNotification(QueuePushNotificationRequest message);
        Task QueueTelegramMessage(QueueTelegramMessageRequest message);
        Task ValidatePushNotificationsSent();
    }
}
