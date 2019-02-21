using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public class Notification<TPayload>
    {
        public string Id { get; }

        public DateTime CreatedAtUtc { get; }

        public TPayload Payload { get; }

        public Notification(TPayload payload)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedAtUtc = DateTime.UtcNow;
            this.Payload = payload;
        }
    }

    public class Notification : Notification<object>
    {
        public Notification(object payload) : base(payload)
        {
        }
    }
}
