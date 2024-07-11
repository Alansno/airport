using airport.Models;
using airport.Models.Dto;
using airport.Models.Querys;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace airport.Services.Interfaces
{
    public interface IFlightService
    {
        public Task<bool> createFlight(FlightDto flightdto);
        public Task<IEnumerable<object>> currentFlights(DateTime dateUser, string timezoneId);
        public Task<IEnumerable<object>> findFlightById(int id);
        public Task<bool> updateFlight(FlightDto flightdto, int id);
        public Task<bool> deleteFlight(int id);
        public Task<FlightReservations> getReservationsById(int id);
    }
}
