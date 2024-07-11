using airport.Models.Dto;

namespace airport.Services.Interfaces
{
    public interface IPlanService
    {
        public Task<bool> createPlan(PlanDto planDto);

    }
}
