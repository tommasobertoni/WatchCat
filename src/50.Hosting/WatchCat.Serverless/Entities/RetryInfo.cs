using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using WatchCat.Core;

namespace WatchCat.Serverless
{
    public class RetryInfo : TableEntity
    {
        public Notification Notification { get; }

        public Exception InitialException { get; }

        public string HandlerId { get; set; }

        public DateTime? LastRetryDateTime { get; set; }

        public RetryInfo(Notification notification, Exception initialException)
        {
            this.Notification = notification;
            this.InitialException = initialException;

            this.PartitionKey = nameof(RetryInfo).ToLower();
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}
