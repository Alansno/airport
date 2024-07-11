namespace airport.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public byte[] Salt { get; set; }
        public required DateTime CreatedAt { get; set; }
        public List<Flight> Flights { get; } = [];
        public List<Reservation> Reservations { get; } = [];
    }
}
