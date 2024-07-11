using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace airport.Models.Dto
{
    public class FlightDto
    {
        [Required]
        public string Origin { get; set; }
        [Required]
        public string Destination { get; set; }
        [Required]
        public DateTime DateDeparture { get; set; }
        [Required]
        public DateTime ArrivalDate { get; set; }
        [Required]
        public string Timezone { get; set; }
        [Required]
        public string Airline { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public string numFlight { get; set; }
        [Required]
        public DateTime currentDate { get; set; }
    }
}
