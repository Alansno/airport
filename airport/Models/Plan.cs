using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace airport.Models
{
    public class Plan : SoftDeleteBase
    {
        public int Id { get; set; }
        public required string TypePlan { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Price { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public required string Description { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null;
    }
}
