using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BBDMS.Web.Hubs
{
    // Emergency Alert Realtime Hub
    public class EmergencyHub : Hub
    {
        public const string DispatcherGroup = "dispatchers";

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole(BBDMS.Web.Filters.Roles.Admin) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, DispatcherGroup);
            }

            await base.OnConnectedAsync();
        }
    }
}
