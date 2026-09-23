using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Data;
using RideMatchingSystem.api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace RideMatchingSystem.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RidesController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IMemoryCache _cache;

        public RidesController(AppDbContext context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // Create ride request
        [HttpPost]
        public async Task<IActionResult> RequestRide(Ride ride)
        {
            ride.Status = "Requested";
            ride.CreatedAt = DateTime.UtcNow;

            _context.Rides.Add(ride);
            await _context.SaveChangesAsync();

            return Ok(ride);
        }

        // Match nearest driver
        [HttpPost("{id}/match")]
        public async Task<IActionResult> MatchRide(int id)
        {
            var ride = await _context.Rides.FindAsync(id);

            if (ride == null)
                return NotFound("Ride not found");

            if (ride.Status != "Requested")
                return BadRequest("Ride is already processed");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var drivers = await _cache.GetOrCreateAsync(
                            "online-drivers",
                            async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow =
                                    TimeSpan.FromSeconds(10);

                                return await _context.Drivers
                                    .Where(d => d.IsOnline)
                                    .ToListAsync();
                            });

            if (!drivers.Any())
                return BadRequest("No online drivers available");

            var nearestDriver = drivers
                .OrderBy(d => Math.Sqrt(
                    Math.Pow(d.Latitude - ride.PickupLatitude, 2) +
                    Math.Pow(d.Longitude - ride.PickupLongitude, 2)))
                .First();

            ride.DriverId = nearestDriver.Id;
            ride.Status = "DriverAssigned";

            nearestDriver.IsOnline = false;

            _cache.Remove("online-drivers");

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(ride);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRide(int id)
        {
            var ride = await _context.Rides.FindAsync(id);

            if (ride == null)
                return NotFound("Ride not found");

            return Ok(ride);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteRide(int id)
        {
            var ride = await _context.Rides.FindAsync(id);

            if (ride == null)
                return NotFound("Ride not found");

            if (ride.Status != "DriverAssigned")
                return BadRequest("Ride cannot be completed");

            ride.Status = "Completed";

            await _context.SaveChangesAsync();

            return Ok(ride);
        }
    }
}