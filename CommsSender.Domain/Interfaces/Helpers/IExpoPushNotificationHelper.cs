using CommsSender.Domain.DTOs.Expo;

namespace CommsSender.Domain.Interfaces.Helpers
{
    public interface IExpoPushNotificationHelper
    {
        Task<ExpoPushReceiptResponse> GetPushReceipts(List<string> receiptIds);
        Task<ExpoPushTicketResponse> SendPushNotification(string expoPushToken, string title, string body, object? data = null, string? channelId = null);
    }
}
