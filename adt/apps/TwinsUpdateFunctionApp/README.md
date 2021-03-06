# Consuming Azure Digital Twins data updates using Event Hub and Function App

## Overview

In this post we will see how easy it is to consume Azure Digital Twin data using Azure Function through Azure Event Hubs. Once the Azure Digital Twin is getting data from IoT devices through Azure IoT Hub, the flow is to read that data in form of Azure Digital Twin graph representation. This post addresses how to read Azure Digital Twin data for the down stream systems.

## Data Flow

![Data Flow](./images/twins-update-data-flow.png "Data Flow")

## Prerequisites

- Azure Subcription
- Admin Access to Azure AD Tenant & Azure Subscription
- Mac OS: [PowerShell for Mac](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-6 )
- Windows OS: PowerShell is built-in
- Azure Digital Twin Instance. The assumption is that the Azure Digital Twin is getting data from IoT device through Azure IoT Hub. For details how you can set up Azure Digital Twin with Azure IoTHub visit [Azure Digital Twins update](https://github.com/nabeelmsft/percept/tree/main/adt/apps/TwinIngestionFunctionApp)


## Reading Azure Digital Twin updates

### Azure Digital Twin setup for routing events to event hub

Routing events from Azure Digital Twin to Azure Event hub is a two step process.

#### Create Azure Digital Twin Endpoint

![Create endpoint](./images/twins-update-create-endpoint.png "Create endpoint")

#### Create Azure Digital Twin Event Route

![Create event route](./images/twins-update-create-eventroute.png "Create event route")

### Function code

```csharp
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

```

Let us take a look at the code.

- The flow starts when event hub triggers this function.
- On the trigger we get a list of events.
- For each event date (part of events list), we are reading the message body.
- Each message body is deserialized to get jToken for each data field.

The Twins Update Function App stops the flow there to create a open for bridging with other down stream systems. One example could be a frontend platform that shows the Azure Digital Twin updates data on a view.

## Conclusion

Using Azure Event Hub and event routing we can route Azure Digital twin events and telemetry to any downstream system.
