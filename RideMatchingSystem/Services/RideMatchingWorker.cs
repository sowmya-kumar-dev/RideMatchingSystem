using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Data;
using Microsoft.AspNetCore.SignalR;
using RideMatchingSystem.api.Hubs;

namespace RideMatchingSystem.api.Services
{
    public class RideMatchingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IHubContext<RideHub> _hubContext;

       
        public RideMatchingWorker( IServiceScopeFactory scopeFactory,IHubContext<RideHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var rides = await context.Rides
                    .Where(r => r.Status == "Requested")
                    .ToListAsync(stoppingToken);

                foreach (var ride in rides)
                {
                    var drivers = await context.Drivers
                        .Where(d => d.IsOnline)
                        .ToListAsync(stoppingToken);

                    if (!drivers.Any())
                        continue;

                    var nearestDriver = drivers
                        .OrderBy(d => Math.Sqrt(
                            Math.Pow(d.Latitude - ride.PickupLatitude, 2) +
                            Math.Pow(d.Longitude - ride.PickupLongitude, 2)))
                        .First();

                    ride.DriverId = nearestDriver.Id;
                    ride.Status = "DriverAssigned";

                    nearestDriver.IsOnline = false;

                    await context.SaveChangesAsync(stoppingToken);

                    await _hubContext.Clients
                        .Group($"driver-{nearestDriver.Id}")
                        .SendAsync(
                            "RideAssigned",
                            new
                            {
                                rideId = ride.Id,
                                driverId = nearestDriver.Id,
                                message = "New ride assigned"
                            },
                            stoppingToken);
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}