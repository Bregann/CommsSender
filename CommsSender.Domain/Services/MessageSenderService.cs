using CommsSender.Domain.Database.Context;
using CommsSender.Domain.Database.Models;
using CommsSender.Domain.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.Services
{
    public class MessageSenderService(AppDbContext context) : IMessageSenderService
    {
        public async Task ProcessPendingMessages()
        {
            // Implementation for processing pending messages

        }

        private async Task SendTelegramMessage(Message message)
        {
                        // Implementation for sending a Telegram message
        }

        private async Task SendPushNotification(Message message)
        {
                        // Implementation for sending a push notification
        }
    }
}
