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
        private readonly INotificationDispatcher _notificationDispatcher;

        public NotificationChannelsController(INotificationDispatcher notificationDispatcher)
        {
            _notificationDispatcher = notificationDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> Post(string message)
        {
            if (message != null && _notificationDispatcher != null)
            {
                await Task.WhenAll(
                    _notificationDispatcher.DispatchAsync(new Notification<object>($"[obj] {message}")),
                    _notificationDispatcher.DispatchAsync(new Notification<string>($"[str] {message}")));
            }

            return Ok();
        }
    }
}