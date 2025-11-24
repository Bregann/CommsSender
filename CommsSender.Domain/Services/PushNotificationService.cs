using CommsSender.Domain.Database.Context;
using CommsSender.Domain.Database.Models;
using CommsSender.Domain.Interfaces.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.Services
{
    public class PushNotificationService(AppDbContext context) : IPushNotificationService
    {
        public async Task RegisterPushToken(string token)
        {
            await context.PushTokens.AddAsync(new PushToken
            {
                Token = token,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            Log.Information("Registered new push token: {Token}", token);
        }
    }
}
