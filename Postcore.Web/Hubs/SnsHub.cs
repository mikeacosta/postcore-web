using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Postcore.Web.Hubs
{
    public class SnsHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
