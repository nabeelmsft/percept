namespace TwinsDashboardApp.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using TwinsDashboardApp.Model;

    public class DashboardHub : Hub
    {
        private TwinData _twinData;
        public DashboardHub(TwinData twinData)
        {
            _twinData = twinData;
            System.Diagnostics.Debug.WriteLine("Received hub");
        }
        public async Task SendMessage(string user, string message)
        {
            _twinData.IncomingData.Add(message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        
        public async Task SendTwinMessage(string floorId, string floorName, string label, string confidence, string timeStamp)
        {
            await Clients.All.SendAsync("ReceiveTwinMessage", floorId, floorName, label, confidence, timeStamp);
        }
    }
}
