using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Core
{
    public class Notification
    {
        public string Id { get; }

        public DateTime CreatedAtUtc { get; }

        public string Message { get; }

        public List<NotificationItem> Items { get; } = new List<NotificationItem>();

        public Notification(string message, IEnumerable<NotificationItem> items = null)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedAtUtc = DateTime.UtcNow;
            this.Message = message;

            if (items != null)
                this.Items.AddRange(items);
        }
    }

    public class NotificationItem
    {
        public string Id { get; }

        public DateTime CreatedAtUtc { get; }

        public string Content { get; }

        public NotificationItem(string content, string id = null, DateTime? createdAtUtc = null)
        {
            this.Id = id;
            this.CreatedAtUtc = createdAtUtc ?? DateTime.UtcNow;
            this.Content = content;
        }
    }
}
