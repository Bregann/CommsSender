using System.Text.Json.Serialization;

namespace CommsSender.Domain.DTOs.Expo
{
    public class ExpoPushMessage
    {
        /// <summary>
        /// Expo push token specifying the recipient of this message
        /// </summary>
        [JsonPropertyName("to")]
        public required string To { get; set; }

        /// <summary>
        /// The title to display in the notification
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The message to display in the notification
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>
        /// A JSON object delivered to your app (up to 4KiB)
        /// </summary>
        [JsonPropertyName("data")]
        public object? Data { get; set; }

        /// <summary>
        /// Time to Live: number of seconds for which the message may be kept for redelivery
        /// </summary>
        [JsonPropertyName("ttl")]
        public int? Ttl { get; set; }

        /// <summary>
        /// The delivery priority of the message (default, normal, or high)
        /// </summary>
        [JsonPropertyName("priority")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpoPriority? Priority { get; set; }

        /// <summary>
        /// Android: ID of the Notification Channel through which to display this notification
        /// </summary>
        [JsonPropertyName("channelId")]
        public string? ChannelId { get; set; }

        /// <summary>
        /// Android: The notification's icon (name of an Android drawable resource)
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        /// <summary>
        /// ID of the notification category that this notification is associated with
        /// </summary>
        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }
    }

    public enum ExpoPriority
    {
        [JsonPropertyName("default")]
        Default,
        [JsonPropertyName("normal")]
        Normal,
        [JsonPropertyName("high")]
        High
    }

    public class ExpoPushTicketResponse
    {
        [JsonPropertyName("data")]
        public List<ExpoPushTicket>? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<ExpoError>? Errors { get; set; }
    }

    public class ExpoPushTicket
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("details")]
        public ExpoErrorDetails? Details { get; set; }
    }

    public class ExpoPushReceiptResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, ExpoPushReceipt>? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<ExpoError>? Errors { get; set; }
    }

    public class ExpoPushReceipt
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("details")]
        public ExpoErrorDetails? Details { get; set; }
    }

    public class ExpoError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class ExpoErrorDetails
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
