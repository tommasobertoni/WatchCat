using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Notifications
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync<TPayload>(Notification<TPayload> notification);
    }
}
