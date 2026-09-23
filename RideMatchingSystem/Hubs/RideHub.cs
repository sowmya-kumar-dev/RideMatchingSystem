using Microsoft.AspNetCore.SignalR;

namespace RideMatchingSystem.api.Hubs
{
    public class RideHub : Hub
    {
        public async Task JoinDriver(int driverId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"driver-{driverId}");
        }
    }
}