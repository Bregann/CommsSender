namespace CommsSender.Domain.DTOs.Messages
{
    public class QueuePushNotificationRequest
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }
}
