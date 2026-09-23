using Microsoft.AspNetCore.Mvc;
using RideMatchingSystem.api.Data;
using RideMatchingSystem.api.Models;

namespace RideMatchingSystem.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RidersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RidersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRider(Rider rider)
        {
            _context.Riders.Add(rider);
            await _context.SaveChangesAsync();

            return Ok(rider);
        }
    }
}