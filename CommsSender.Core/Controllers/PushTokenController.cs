using CommsSender.Domain.Interfaces.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CommsSender.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PushTokenController(IPushNotificationService pushNotificationService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RegisterPushToken([FromBody] string pushToken)
        {
            await pushNotificationService.RegisterPushToken(pushToken);
            return Ok(true);
        }
    }
}
