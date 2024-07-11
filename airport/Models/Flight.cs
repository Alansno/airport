
namespace airport.Models
{
    public class Flight : SoftDeleteBase
    {
        public int Id { get; set; }
        public required string Origin { get; set; }
        public required string Destination { get; set; }
        public required DateTime DateDeparture { get; set; }
        public required DateTime ArrivalDate { get; set; }
        public required string Status { get; set; } = "Programado";
        public required string Airline { get; set; }
        public required int Capacity { get; set; }
        public required string numFlight { get; set; }
        public ICollection<Plan> Plans { get; } = new List<Plan>();
        public List<User> Users { get; } = [];
        public List<Reservation> Reservations { get; } = [];
    }
}
