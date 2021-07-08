using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TwinsUpdateBetaFunctionApp.Model;

namespace TwinsUpdateBetaFunctionApp
{
    public static class TwinsUpdateBetaFunction
    {
        [FunctionName("TwinsUpdateBetaFunction")]
        public static void Run([EventHubTrigger("santacruz3203-digitaltwin-eventhub", Connection = "EventHubConnection")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            foreach (EventData eventData in events)
            {
                try
                {
                    log.LogInformation($"Message received: {eventData.EventBody}");

                    // Deserialize the message into TwinMessage object. TwinMessage is a custom class representing the schema of Azure Digital Twins message.
                    TwinMessage twinMessage = eventData.EventBody.ToObjectFromJson<TwinMessage>();

                    // Here add your code to further process TwinMessage.

                    log.LogInformation($"Message processed");
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
