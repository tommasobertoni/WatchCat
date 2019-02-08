using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public class MessageNotification : Notification
    {
        public string Message { get; }

        public MessageNotification(string message) : base()
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            this.Message = message;
        }
    }
}
