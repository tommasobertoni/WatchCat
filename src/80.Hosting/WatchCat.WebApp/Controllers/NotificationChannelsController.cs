using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCat.Core.Notifications;

namespace WatchCat.WebApp.Controllers
{
    [Route("api/Notifications/Channels")]
    [ApiController]
    public class NotificationChannelsController : Controller
    {
        private readonly INotificationChannel _notificationChannel;

        public NotificationChannelsController(INotificationChannel notificationChannel)
        {
            _notificationChannel = notificationChannel;
        }

        [HttpGet]
        public string? Get()
        {
            return _notificationChannel?.Description ?? null;
        }

        [HttpPost]
        public async Task<ActionResult> Post(string message)
        {
            if (message != null && _notificationChannel != null)
                await _notificationChannel.NotifyAsync(new MessageNotification(message));

            return Ok();
        }
    }
}