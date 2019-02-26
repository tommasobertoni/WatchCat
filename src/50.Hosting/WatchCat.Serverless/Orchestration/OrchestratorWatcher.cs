using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WatchCat.Serverless.Orchestration
{
    public class OrchestratorWatcher
    {
        protected readonly DurableOrchestrationClient _client;
        private readonly int _pollingDelayMillis;
        private readonly int _orchestratorStartTimeoutMillis;

        public OrchestratorWatcher(
            DurableOrchestrationClient client,
            int pollingDelayMillis = 500,
            int orchestratorStartTimeoutMillis = 5000)
        {
            _client = client;
            _pollingDelayMillis = pollingDelayMillis;
            _orchestratorStartTimeoutMillis = orchestratorStartTimeoutMillis;
        }

        public virtual Task<bool> FunctionHasActiveStatusAsync(string instanceId)
        {
            var functionIsRunningTcs = new TaskCompletionSource<bool>();

            Task.Run(async () =>
            {
                using (var cts = new CancellationTokenSource())
                {
                    var pollingStatusTask = PollUntilActiveStatusAsync(instanceId, cts.Token);
                    var timeoutTask = Task.Delay(_orchestratorStartTimeoutMillis);

                    var functionIsActive = await Task.WhenAny(pollingStatusTask, timeoutTask) == pollingStatusTask;
                    cts.Cancel();

                    functionIsRunningTcs.TrySetResult(functionIsActive);
                }
            });

            return functionIsRunningTcs.Task;
        }

        protected virtual async Task<bool> PollUntilActiveStatusAsync(string instanceId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var orchestratorStatus = await _client.GetStatusAsync(instanceId);

                if (orchestratorStatus != null && orchestratorStatus.RuntimeStatus != OrchestrationRuntimeStatus.Pending)
                    return true;

                await Task.Delay(_pollingDelayMillis);
            }

            return !cancellationToken.IsCancellationRequested;
        }
    }
}
