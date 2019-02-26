using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WatchCat.Serverless.Components
{
    public class Storage
    {
        internal static readonly string AzureStorageValueKey = "AzureWebJobsStorage";

        private readonly Configuration _configuration;

        private readonly CloudStorageAccount _storageAccount;

        public Storage(Configuration configuration, string connectionString = null)
        {
            _configuration = configuration;

            if (connectionString == null)
                connectionString = Environment.GetEnvironmentVariable(AzureStorageValueKey);

            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<CloudQueue> GetBufferQueueAsync(bool createIfNotExists = false)
        {
            var queueClient = _storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_configuration.EventsBufferQueueName);
            if (createIfNotExists)
            {
                bool queueCreated = await queue.CreateIfNotExistsAsync(); 
            }
            return queue;
        }
    }
}
