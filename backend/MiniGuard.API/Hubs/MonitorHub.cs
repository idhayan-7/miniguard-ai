using Microsoft.AspNetCore.SignalR;

namespace MiniGuard.API.Hubs;

public class MonitorHub : Hub
{
    // Clients connect here to receive real-time health updates.
    // The server pushes via IHubContext<MonitorHub> from AnomalyService.
    // Client listens for: "ReceiveHealthUpdate"
}
