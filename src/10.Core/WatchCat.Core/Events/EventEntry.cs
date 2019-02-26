using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Core.Events
{
    public class EventEntry
    {
        public int ApiVersion { get; }

        public string Name { get; set; }

        public object Content { get; }

        public EventEntry(object content = null, string name = null, int apiVersion = 1)
        {
            this.ApiVersion = apiVersion;
            this.Name = name;
            this.Content = content;
        }
    }
}
