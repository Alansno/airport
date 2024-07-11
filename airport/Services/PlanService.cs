using airport.Context;
using airport.Models;
using airport.Models.Dto;
using airport.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace airport.Services
{
    public class PlanService: IPlanService
    {
        private readonly AirportContext _airportContext;
        public PlanService(AirportContext airportContext)
        {
            _airportContext = airportContext;
        }

        public async Task<bool> createPlan(PlanDto planDto)
        {
            var plan = new Plan
            {
                TypePlan = planDto.TypePlan,
                Price = planDto.Price,
                Description = planDto.Description,
                FlightId = planDto.FlightId,
            };

            try
            {
                await _airportContext.Plans.AddAsync(plan);
                await _airportContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
