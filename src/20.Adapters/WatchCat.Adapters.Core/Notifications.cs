using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters
{
    public class PayloadNotification : INotification
    {
        public object Payload { get; }

        public PayloadNotification(object payload)
        {
            this.Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }
    }

    public class MessageNotification : INotification
    {
        public string Message { get; }

        public MessageNotification(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            this.Message = message;
        }
    }
}
