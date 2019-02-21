using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public interface INotificationChannel<TPayload>
    {
        Task HandleAsync(Notification<TPayload> notification);
    }

    public interface INotificationChannel : INotificationChannel<object>
    {
    }
}
