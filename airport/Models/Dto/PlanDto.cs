using System.ComponentModel.DataAnnotations;

namespace airport.Models.Dto
{
    public class PlanDto
    {
        [Required]
        public required string TypePlan { get; set; }
        [Required]
        public required decimal Price { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public int FlightId { get; set; }
    }
}
