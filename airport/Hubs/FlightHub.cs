using airport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace airport.Hubs
{
    [Authorize]
    public class FlightHub : Hub
    {
        private readonly IFlightService _flightService;
        public FlightHub(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public async Task SendFlights(DateTime dateUser, string timezoneId)
        {
            var currentFlights = await _flightService.currentFlights(dateUser, timezoneId);
            await Clients.All.SendAsync("flights", currentFlights);
        }
    }
}
