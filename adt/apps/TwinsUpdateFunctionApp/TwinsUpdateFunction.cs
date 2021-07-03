namespace TwinsUpdateFunctionApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
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
        private static readonly string twinReceiverUrl = Environment.GetEnvironmentVariable("TWINS_RECEIVER_URL");
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

                        using (HttpClient httpClient = new HttpClient())
                        {
                            var requestURl = new Uri($"{twinReceiverUrl}?label={twinUpdate.Label}&confidence={twinUpdate.Confidence}&timestamp={twinUpdate.Timestamp}&floorId={twinUpdate.Floor}&floorName={twinUpdate.FloorName}");
                            StringContent queryString = new StringContent(messageBody);
                            var response = httpClient.PostAsync(requestURl, queryString).Result;
                        }

                        twinUpdates.Add(twinUpdate);
                    }
                    
                    // Add any custom logic to process data.
                    log.LogInformation($"Message received: {messageBody}");
                    
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
