using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Data;

namespace RideMatchingSystem.api.Services
{
    public class RideMatchingService
    {
        private readonly AppDbContext _context;

        public RideMatchingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task MatchPendingRidesAsync()
        {
            var rides = await _context.Rides
                .Where(r => r.Status == "Requested")
                .ToListAsync();

            foreach (var ride in rides)
            {
                var drivers = await _context.Drivers
                    .Where(d => d.IsOnline)
                    .ToListAsync();

                if (!drivers.Any())
                    continue;

                var nearestDriver = drivers
                    .OrderBy(d => CalculateDistance(
                        ride.PickupLatitude,
                        ride.PickupLongitude,
                        d.Latitude,
                        d.Longitude))
                    .First();

                // Mark driver unavailable
                nearestDriver.IsOnline = false;

                // Assign driver
                ride.DriverId = nearestDriver.Id;
                ride.Status = "DriverAssigned";

                await _context.SaveChangesAsync();
            }
        }

        private static double CalculateDistance(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            return Math.Sqrt(
                (dLat * dLat) +
                (dLon * dLon));
        }
    }
}