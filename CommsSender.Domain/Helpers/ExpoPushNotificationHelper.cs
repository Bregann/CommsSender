using CommsSender.Domain.DTOs.Expo;
using CommsSender.Domain.Interfaces.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommsSender.Domain.Helpers
{
    public class ExpoPushNotificationHelper(HttpClient httpClient) : IExpoPushNotificationHelper
    {
        private const string ExpoApiUrl = "https://exp.host/--/api/v2/push/send";

        public async Task<ExpoPushTicketResponse> SendPushNotification(
            string expoPushToken, 
            string title, 
            string body, 
            object? data = null,
            string? channelId = null)
        {
            var message = new ExpoPushMessage
            {
                To = expoPushToken,
                Title = title,
                Body = body,
                Data = data,
                ChannelId = channelId,
                Priority = ExpoPriority.High
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ExpoApiUrl)
            {
                Content = JsonContent.Create(message)
            };

            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ExpoPushTicketResponse>();
            return result ?? throw new InvalidOperationException("Failed to deserialize response");
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
