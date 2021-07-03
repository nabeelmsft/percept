namespace TwinsDashboardApp.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using TwinsDashboardApp.Hubs;
    using TwinsDashboardApp.model;
    using TwinsDashboardApp.Model;

    [IgnoreAntiforgeryToken]
    public class TwinReceiverModel : PageModel
    {
        private readonly ILogger<TwinReceiverModel> _logger;
        private IHubContext<DashboardHub> _dashboardHub;


        public TwinReceiverModel(ILogger<TwinReceiverModel> logger, IHubContext<DashboardHub> dashboardHub)
        {
            _logger = logger;
            _dashboardHub = dashboardHub;
        }

        public async Task OnPostAsync()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var message = await reader.ReadToEndAsync();
                JObject twinMessage = (JObject)JsonConvert.DeserializeObject(message);
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

                    await _dashboardHub.Clients.All.SendAsync("ReceiveTwinMessage", twinUpdate.Floor, twinUpdate.FloorName, twinUpdate.Label, twinUpdate.Confidence, twinUpdate.Timestamp, message);
                }
            }
        }
    }
}
