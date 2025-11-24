using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.Interfaces.Helpers
{
    public interface IPushNotificationService
    {
        Task RegisterPushToken(string token);
    }
}
