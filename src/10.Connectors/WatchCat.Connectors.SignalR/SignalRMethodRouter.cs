using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchCat.Connectors.SignalR
{
    public class SignalRMethodRouter
    {
        public string Method { get; }

        public HashSet<Type> AcceptedMessageTypes { get; } = new HashSet<Type>();

        public SignalRMethodRouter(string method, IEnumerable<Type> acceptedMessageTypes = null)
        {
            this.Method = method;

            if (acceptedMessageTypes != null)
                foreach (var type in acceptedMessageTypes)
                    AcceptedMessageTypes.Add(type);
        }
    }
}
