using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Serverless.Orchestration
{
    // Workaround for running a singleton orchestrator
    // ref: https://github.com/Azure/azure-functions-durable-extension/issues/612

    public class OrchestratorFactory
    {
        protected static readonly string AzureStorageValueKey = "AzureWebJobsStorage";

        protected static readonly OrchestrationRuntimeStatus[] _completedStatuses = new[]
        {
            OrchestrationRuntimeStatus.Completed,
            OrchestrationRuntimeStatus.Failed,
            OrchestrationRuntimeStatus.Terminated,
            OrchestrationRuntimeStatus.Canceled
        };

        private readonly CloudTable _supportTable;
        protected readonly ILogger _log;

        private OrchestratorFactory(CloudTable supportTable, ILogger log = null)
        {
            _supportTable = supportTable;
            _log = log;
        }

        public static Task<OrchestratorFactory> CreateAsync(string connectionString = null, ILogger log = null)
        {
            if (connectionString == null)
                connectionString = Environment.GetEnvironmentVariable(AzureStorageValueKey);

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var supportTable = tableClient.GetTableReference("SingletonFunctionsSupportTableRegistry");

            return CreateAsync(supportTable, log);
        }

        public static async Task<OrchestratorFactory> CreateAsync(CloudTable supportTable, ILogger log = null)
        {
            await supportTable.CreateIfNotExistsAsync();
            return new OrchestratorFactory(supportTable, log);
        }

        public async Task<bool> TryStartNewAsync(
            DurableOrchestrationClient client,
            string orchestratorFunctionName,
            string instanceId,
            object input,
            bool startIfAlreadyCompleted = false)
        {
            // Check if an instance with the specified ID already exists.
            var existingInstance = await client.GetStatusAsync(instanceId);
            if (existingInstance != null)
            {
                bool restartFunction = startIfAlreadyCompleted && _completedStatuses.Contains(existingInstance.RuntimeStatus);

                if (!restartFunction)
                    return false; // Function exists and should not be restarted.
            }

            // Create a temporary table record to track the creation of the current function.
            var recordCreated = await TryCreateRecordAsync(orchestratorFunctionName, instanceId);

            if (!recordCreated)
            {
                _log?.LogWarning($"Record already exists, possible concurrent start of the orchestrator {orchestratorFunctionName} with id {instanceId}.");
                return false;
            }

            if (existingInstance != null)
                await TryClearFunctionHistoryAsync(client, instanceId);

            // Since the record didn't exist, we can *safely* create the singleton orchestrator
            var functionId = await client.StartNewAsync(orchestratorFunctionName, instanceId, input);

            // Await the function to start
            var watcher = new OrchestratorWatcher(client);
            var functionIsActive = await watcher.FunctionHasActiveStatusAsync(functionId);

            await TryDeleteRecordAsync(orchestratorFunctionName, instanceId);

            return true;
        }

        private async Task<bool> TryClearFunctionHistoryAsync(DurableOrchestrationClient client, string instanceId)
        {
            try
            {
                await client.PurgeInstanceHistoryAsync(instanceId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual async Task<bool> TryCreateRecordAsync(string orchestratorFunctionName, string instanceId)
        {
            try
            {
                var tableResult = await _supportTable.ExecuteAsync(
                    TableOperation.Insert(
                        new OrchestratorCreationTableEntry(orchestratorFunctionName, instanceId)));

                return true;
            }
            catch (StorageException storageEx) when (storageEx.RequestInformation?.HttpStatusCode == 409 /* Conflict */)
            {
                return false;
            }
            catch (Exception ex)
            {
                _log?.LogError(ex, ex.Message);
                return false;
            }
        }

        protected virtual async Task<bool> TryDeleteRecordAsync(string orchestratorFunctionName, string instanceId)
        {
            try
            {
                var tableResult = await _supportTable.ExecuteAsync(
                        TableOperation.Delete(
                            new OrchestratorCreationTableEntry(orchestratorFunctionName, instanceId) { ETag = "*" }));

                return true;
            }
            catch (Exception ex)
            {
                _log?.LogError(ex, ex.Message);
                return false;
            }
        }

        private class OrchestratorCreationTableEntry : TableEntity
        {
            public OrchestratorCreationTableEntry(string orchestratorName, string instanceId)
            {
                this.PartitionKey = orchestratorName;
                this.RowKey = instanceId;
            }
        }
    }
}
