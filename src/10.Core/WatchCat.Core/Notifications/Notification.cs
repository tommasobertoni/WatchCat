using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public abstract class Notification
    {
        public virtual string Id { get; }

        public DateTime CreatedAtUtc { get; }

        public Notification()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
