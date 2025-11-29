using CommsSender.Domain.DTOs.Messages;
using CommsSender.Domain.Interfaces.Api;
using Microsoft.AspNetCore.Mvc;

namespace CommsSender.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController(IMessageService messageService) : ControllerBase
    {
        [HttpPost]
        public IActionResult SendTelegramMessage([FromBody] QueueTelegramMessageRequest request)
        {
            messageService.QueueTelegramMessage(request);
            return Ok(true);
        }

        [HttpPost]
        public IActionResult SendPushNotification([FromBody] QueuePushNotificationRequest request)
        {
            messageService.QueuePushNotification(request);
            return Ok(true);
        }
    }
}
