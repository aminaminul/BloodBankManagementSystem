using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BBDMS.Web.Hubs
{
    public class EmergencyHub : Hub
    {
        public async Task BroadcastBloodRequest(string patientName, string bloodGroup, string hospital, string urgency)
        {
            await Clients.All.SendAsync("ReceiveBloodRequestAlert", new
            {
                patientName,
                bloodGroup,
                hospital,
                urgency,
                timestamp = System.DateTime.Now.ToString("hh:mm tt")
            });
        }

        public async Task BroadcastAmbulanceRequest(string patientName, string location, string contact, string ambulanceType)
        {
            await Clients.All.SendAsync("ReceiveAmbulanceAlert", new
            {
                patientName,
                location,
                contact,
                ambulanceType,
                timestamp = System.DateTime.Now.ToString("hh:mm tt")
            });
        }
    }
}
