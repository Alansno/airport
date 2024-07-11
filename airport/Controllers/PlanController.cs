using airport.Models.Dto;
using airport.Models.Response;
using airport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace airport.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpPost]
        [Authorize(Roles = "Encargado")]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] PlanDto planDto)
        {
            var response = new Response();
            response.Success = false;

            try
            {
                var plan = await _planService.createPlan(planDto);
                response.Success = true;
                response.Message = "Plan creado correctamente";
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Algo salió mal");
            }
            return Ok(response);
        }
    }
}
