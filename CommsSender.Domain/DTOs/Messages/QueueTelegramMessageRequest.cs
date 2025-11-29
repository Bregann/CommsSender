namespace CommsSender.Domain.DTOs.Messages
{
    public class QueueTelegramMessageRequest
    {
        public required long ChatId { get; set; }
        public required string MessageText { get; set; }
    }
}
