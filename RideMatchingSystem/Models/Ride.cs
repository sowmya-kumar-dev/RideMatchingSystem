namespace RideMatchingSystem.api.Models
{
    public class Ride
    {
        public int Id { get; set; }
        public int RiderId { get; set; }
        public int? DriverId { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DropLatitude { get; set; }
        public double DropLongitude { get; set; }
        public string Status { get; set; } = "Requested";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
