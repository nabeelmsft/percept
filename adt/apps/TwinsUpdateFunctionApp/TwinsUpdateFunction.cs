namespace TwinsUpdateFunctionApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using TwinsUpdateFunctionApp.model;

    public static class TwinsUpdateFunction
    {
        [FunctionName("TwinsUpdateFunction")]
        public static async Task Run([EventHubTrigger("santacruz3203-digitaltwin-eventhub", Connection = "EventHubConnectionString")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            List<TwinUpdate> twinUpdates = new List<TwinUpdate>();
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    JObject twinMessage = (JObject)JsonConvert.DeserializeObject(messageBody);
                    if (twinMessage["patch"] != null)
                    {
                        TwinUpdate twinUpdate = new TwinUpdate();
                        twinUpdate.ModelId = twinMessage["modelId"].ToString();
                        foreach (JToken jToken in twinMessage["patch"])
                        {
                            if (jToken["path"].ToString().Equals("/FloorId", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Floor = jToken["value"].ToString();
                            }
                            if (jToken["path"].ToString().Equals("/FloorName", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.FloorName = jToken["value"].ToString();
                            }
                            if (jToken["path"].ToString().Equals("/Label", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Label = jToken["value"].ToString();
                            }
                            if (jToken["path"].ToString().Equals("/Confidence", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Confidence = jToken["value"].ToString();
                            }
                            if (jToken["path"].ToString().Equals("/timestamp", StringComparison.InvariantCultureIgnoreCase))
                            {
                                twinUpdate.Timestamp = jToken["value"].ToString();
                            }
                        }

                        twinUpdates.Add(twinUpdate);
                    }
                        // Replace these two lines with your processing logic.
                        log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
