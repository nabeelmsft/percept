using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwinsUpdateBetaFunctionApp.Model;

namespace TwinsUpdateBetaFunctionApp
{
    public static class TwinsUpdateBetaFunction
    {
        [FunctionName("TwinsUpdateBetaFunction")]
        public static async Task Run([EventHubTrigger("santacruz3203-digitaltwin-eventhub", Connection = "EventHubConnection")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            List<TwinUpdate> twinUpdates = new List<TwinUpdate>();

            foreach (EventData eventData in events)
            {
                try
                {
                    TwinMessage twinMessage = eventData.EventBody.ToObjectFromJson<TwinMessage>();
                    if (twinMessage.patch != null)
                    {
                        TwinUpdate twinUpdate = new TwinUpdate();
                        twinUpdate.ModelId = twinMessage.modelId;
                        foreach (Patch patchItem in twinMessage.patch)
                        {
                            if(patchItem.path.Equals("/FloorId", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Floor = patchItem.value;
                            }

                            if (patchItem.path.Equals("/FloorName", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.FloorName = patchItem.value;
                            }

                            if (patchItem.path.Equals("/Label", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Label = patchItem.value;
                            }

                            if (patchItem.path.Equals("/Confidence", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Confidence = patchItem.value;
                            }

                            if (patchItem.path.Equals("/timestamp", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Timestamp = patchItem.value;
                            }
                        }

                        twinUpdates.Add(twinUpdate);
                    }
                    log.LogInformation($"Message received: {eventData.EventBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
