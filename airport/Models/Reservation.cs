namespace airport.Models
{
    public class Reservation : SoftDeleteBase
    {
        public int Id { get; set; }
        public required DateTime DateReservation { get; set; }
        public required string StateReservation { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public int PlanId { get; set; }
        public User User { get; set; }
        public Flight Flight { get; set; }
        public Plan Plan { get; set; }
    }
}
