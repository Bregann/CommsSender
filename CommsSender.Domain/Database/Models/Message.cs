using CommsSender.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommsSender.Domain.Database.Models
{
    public class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string MessageTitle { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }

        [Required]
        public string Sender { get; set; } = string.Empty;

        [Required]
        public MessageType MessageType { get; set; }

        public long? TelegramChatId { get; set; }

        [Required]
        public MessageStatus Status { get; set; }

        [Required]
        public int MessageSentAttempts { get; set; } = 0;

        public bool? PushNotificationDelivered { get; set; } = null;

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual ICollection<MessageErrorLog> ErrorLogs { get; set; } = null!;
    }
}
