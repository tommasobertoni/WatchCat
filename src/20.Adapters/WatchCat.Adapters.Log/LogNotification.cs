using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchCat.Adapters.Log
{
    public class LogNotification : INotification
    {
        public LogLevel Level { get; }

        public string Message { get; }

        public LogNotification(LogLevel level, string message)
        {
            this.Level = level;
            this.Message = message;
        }
    }
}
