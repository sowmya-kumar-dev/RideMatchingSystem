namespace RideMatchingSystem.api.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsOnline { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
