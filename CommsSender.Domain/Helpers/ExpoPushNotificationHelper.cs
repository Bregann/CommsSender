using CommsSender.Domain.DTOs.Expo;
using CommsSender.Domain.Interfaces.Helpers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommsSender.Domain.Helpers
{
    public class ExpoPushNotificationHelper(HttpClient httpClient) : IExpoPushNotificationHelper
    {
        private const string ExpoApiUrl = "https://exp.host/--/api/v2/push/send";
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public async Task<ExpoPushTicketResponse> SendPushNotification(
            string expoPushToken,
            string title,
            string body)
        {
            var messages = new[]
            {
                new ExpoPushMessage
                {
                    To = expoPushToken,
                    Title = title,
                    Body = body
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ExpoApiUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(messages, JsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStreamAsync();
            var ticket = await JsonSerializer.DeserializeAsync<ExpoPushTicketResponse>(result, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize response");

            return ticket;
        }

        public async Task<ExpoPushReceiptResponse> GetPushReceipts(List<string> receiptIds)
        {
            const string receiptsUrl = "https://exp.host/--/api/v2/push/getReceipts";

            var requestBody = new { ids = receiptIds };
            var request = new HttpRequestMessage(HttpMethod.Post, receiptsUrl)
            {
                Content = JsonContent.Create(requestBody)
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ExpoPushReceiptResponse>();
            return result ?? throw new InvalidOperationException("Failed to deserialize response");
        }
    }
}
