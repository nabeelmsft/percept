namespace TwinsUpdateBetaFunctionApp
{
    using Azure.Messaging.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TwinsUpdateBetaFunctionApp.model;

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
                    JsonElement twinMessage = JsonSerializer.Deserialize<JsonElement>(eventData.EventBody.ToString());
                    var patch = twinMessage.GetProperty("patch");
                    if (!string.IsNullOrEmpty(twinMessage.GetProperty("patch").ToString()))
                    {
                        TwinUpdate twinUpdate = new TwinUpdate();
                        twinUpdate.ModelId = twinMessage.GetProperty("modelId").ToString();
                        foreach (JsonElement patchItem in twinMessage.GetProperty("patch").EnumerateArray())
                        {
                            if (patchItem.GetProperty("path").ToString().Equals("/FloorId", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Floor = patchItem.GetProperty("value").ToString();
                            }

                            if (patchItem.GetProperty("path").ToString().Equals("/FloorName", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.FloorName = patchItem.GetProperty("value").ToString();
                            }

                            if (patchItem.GetProperty("path").ToString().Equals("/Label", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Label = patchItem.GetProperty("value").ToString();
                            }

                            if (patchItem.GetProperty("path").ToString().Equals("/Confidence", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Confidence = patchItem.GetProperty("value").ToString();
                            }

                            if (patchItem.GetProperty("path").ToString().Equals("/timestamp", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Timestamp = patchItem.GetProperty("value").ToString();
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
