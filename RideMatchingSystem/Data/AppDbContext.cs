using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Models;

namespace RideMatchingSystem.api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Rider> Riders { get; set; }
        public DbSet<Ride> Rides { get; set; }
    }
}
