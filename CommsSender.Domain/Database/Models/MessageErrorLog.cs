using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommsSender.Domain.Database.Models
{
    public class MessageErrorLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; } = null!;

        [Required]
        public string ErrorMessage { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
