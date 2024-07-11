using System.ComponentModel.DataAnnotations;

namespace airport.Models.Dto
{
    public class ReservationDto
    {
        [Required]
        public int FlightId { get; set; }
        [Required]
        public int PlanId { get; set; }
    }
}
