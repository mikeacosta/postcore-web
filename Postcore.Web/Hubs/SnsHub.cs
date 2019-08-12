using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Postcore.Web.Hubs
{
    public class SnsHub : Hub
    {
        private readonly ILogger<SnsHub> _logger;

        public SnsHub(ILogger<SnsHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            if (ex != null)
                _logger.LogError($"{nameof(SnsHub)} disconnected: {ex.Message}");

            return base.OnDisconnectedAsync(ex);
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"{nameof(SnsHub)}: connected");
            return base.OnConnectedAsync();
        }
    }
}
