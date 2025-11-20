using System;
using System.Collections.Generic;
using System.Text;

namespace CommsSender.Domain.Interfaces.Api
{
    public interface IMessageSenderService
    {
        Task ProcessPendingMessages();
    }
}
