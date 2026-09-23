using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Data;
using RideMatchingSystem.api.Models;

namespace RideMatchingSystem.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DriversController(AppDbContext context)
        {
            _context = context;
        }

        // Create a driver
        [HttpPost]
        public async Task<IActionResult> CreateDriver(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return Ok(driver);
        }

        // Driver goes online
        [HttpPost("{id}/online")]
        public async Task<IActionResult> GoOnline(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.IsOnline = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver is now online",
                driverId = driver.Id
            });
        }

        // Update driver location
        [HttpPost("{id}/location")]
        public async Task<IActionResult> UpdateLocation(
            int id,
            double latitude,
            double longitude)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.Latitude = latitude;
            driver.Longitude = longitude;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Location updated",
                latitude,
                longitude
            });
        }

        // Driver goes offline
        [HttpPost("{id}/offline")]
        public async Task<IActionResult> GoOffline(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.IsOnline = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver is now offline",
                driverId = driver.Id
            });
        }
    }
}