namespace TwinsDashboardApp.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using TwinsDashboardApp.Hubs;
    using TwinsDashboardApp.Model;

    public class TwinReceiverModel : PageModel
    {
        private readonly ILogger<TwinReceiverModel> _logger;
        private IHubContext<DashboardHub> _dashboardHub;
        private TwinData _twinData;

        public TwinReceiverModel(ILogger<TwinReceiverModel> logger, TwinData twinData, IHubContext<DashboardHub> dashboardHub)
        {
            _twinData = twinData;
            _logger = logger;
            _dashboardHub = dashboardHub;
        }

        public void OnGet(string label, string confidence, string timeStamp, string floorId, string floorName)
        {
            _dashboardHub.Clients.All.SendAsync("ReceiveTwinMessage", floorId, floorName, label, confidence, timeStamp);
        }

    }
}
