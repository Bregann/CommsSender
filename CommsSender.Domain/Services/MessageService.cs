using CommsSender.Domain.Database.Context;
using CommsSender.Domain.Database.Models;
using CommsSender.Domain.DTOs.Messages;
using CommsSender.Domain.Enums;
using CommsSender.Domain.Interfaces.Api;
using CommsSender.Domain.Interfaces.Helpers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace CommsSender.Domain.Services
{
    public class MessageService(AppDbContext context, IEnvironmentalSettingHelper environmentalSettingHelper, IExpoPushNotificationHelper expoPushNotificationHelper) : IMessageService
    {
        private readonly int _batchSize = 10;
        private readonly int _retryLimit = 3;

        public async Task QueueTelegramMessage(QueueTelegramMessageRequest message)
        {
            context.Messages.Add(new Message
            {
                MessageType = MessageType.Telegram,
                TelegramChatId = message.ChatId,
                Content = message.MessageText,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            Log.Information($"Queued Telegram message for ChatId {message.ChatId}. Contents {message.MessageText}");
        }

        public async Task QueuePushNotification(QueuePushNotificationRequest message)
        {
            context.Messages.Add(new Message
            {
                MessageType = MessageType.PushNotification,
                MessageTitle = message.Title,
                Content = message.Body,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            Log.Information($"Queued Push Notification message. Title: {message.Title}, Body: {message.Body}");
        }

        public async Task ProcessPendingMessages()
        {
            var pendingMessages = await context.Messages
                .Where(m => m.Status == MessageStatus.Pending)
                .Take(_batchSize)
                .ToListAsync();

            // get the latest push token
            var token = await context.PushTokens
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => t.Token)
                .FirstAsync();

            foreach (var message in pendingMessages)
            {
                switch (message.MessageType)
                {
                    case MessageType.Telegram:
                        await SendTelegramMessage(message);
                        break;
                    case MessageType.PushNotification:
                        await SendPushNotification(message, token);
                        break;
                }
            }

        }

        private async Task SendTelegramMessage(Message message)
        {
            try
            {
                var telegramApiKey = environmentalSettingHelper.TryGetEnviromentalSettingValue(EnvironmentalSettingEnum.TelegramBotApiKey) ?? throw new Exception("Telegram API Key not configured");
                var botClient = new TelegramBotClient(telegramApiKey);

                using var cts = new CancellationTokenSource();

                var sentMessage = await botClient.SendMessage(
                    chatId: message.TelegramChatId ?? throw new Exception("Telegram Chat ID missing"),
                    text: message.Content);

                cts.Cancel();

                message.Status = MessageStatus.Sent;
                message.SentAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                Log.Information($"Telegram message id {message.Id} processed");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error sending Telegram message id {message.Id}");

                message.MessageSentAttempts += 1;

                if (message.MessageSentAttempts >= _retryLimit)
                {
                    message.Status = MessageStatus.Failed;
                }

                await context.MessageErrorLogs.AddAsync(new MessageErrorLog
                {
                    MessageId = message.Id,
                    ErrorMessage = e.Message,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
                return;
            }
        }

        private async Task SendPushNotification(Message message, string pushToken)
        {
            await expoPushNotificationHelper.SendPushNotification(
                expoPushToken: pushToken,
                title: message.MessageTitle ?? string.Empty,
                body: message.Content
            );

            message.Status = MessageStatus.Sent;
            message.SentAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            Log.Information($"Push Notification message id {message.Id} processed");
        }
    }
}
