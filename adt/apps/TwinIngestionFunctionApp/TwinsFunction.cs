namespace TwinInjestionFunctionApp
{
    using Azure;
    using Azure.Core.Pipeline;
    using Azure.DigitalTwins.Core;
    using Azure.Identity;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Linq;
    using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

    public class TwinsFunction
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly string labelToBeDetected = Environment.GetEnvironmentVariable("TARGET_LABEL");
        private static readonly string ConfidenceThreshold = Environment.GetEnvironmentVariable("TARGET_CONFIDENCE_THRESHOLD");
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("TwinsFunction")]
        public async void Run([IoTHubTrigger("messages/events", Connection = "EventHubConnectionString")] EventData message, ILogger log)
        {
            if (adtInstanceUrl == null) log.LogError("Application setting \"ADT_SERVICE_URL\" not set");
            {
                try
                {
                    //Authenticate with Digital Twins
                    ManagedIdentityCredential cred = new ManagedIdentityCredential("https://digitaltwins.azure.net");
                    DigitalTwinsClient client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred, new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });
                    if (message != null && message.Body != null)
                    {
                        // Reading AI data for IoT Hub JSON
                        JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body.Array));
                        if (deviceMessage["NEURAL_NETWORK"] != null)
                        {
                            foreach (var neuralActivity in deviceMessage["NEURAL_NETWORK"])
                            {
                                string label = neuralActivity["label"].ToString();
                                string confidence = neuralActivity["confidence"].ToString();
                                string timestamp = neuralActivity["timestamp"].ToString();

                                // Ensuring we are getting valid values for label, confidence and timestamp.
                                if (!(string.IsNullOrEmpty(label) && string.IsNullOrEmpty(confidence) && string.IsNullOrEmpty(timestamp)))
                                {
                                    // Check to see if the object that is detected is the one that we want the Azure Digital Twin is updated with. Also ensuring the confidence value is greater than threshold.
                                    if (labelToBeDetected.Split(",").Contains<string>(label, StringComparer.InvariantCultureIgnoreCase) && Double.Parse(confidence) > Double.Parse(ConfidenceThreshold))
                                    {
                                        var updateTwinData = new JsonPatchDocument();
                                        updateTwinData.AppendAdd("/FloorId", "1");
                                        updateTwinData.AppendAdd("/FloorName", "PerceptSiteFloor");
                                        updateTwinData.AppendAdd("/Label", label);
                                        updateTwinData.AppendAdd("/Confidence", confidence);
                                        updateTwinData.AppendAdd("/timestamp", timestamp);
                                        await client.UpdateDigitalTwinAsync("PerceptSiteFloor", updateTwinData);
                                        log.LogInformation($"Updated Device: PerceptSiteFloor with { updateTwinData} at: {DateTime.Now.ToString()}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                }

            }
        }
    }
}