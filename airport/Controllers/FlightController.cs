using airport.Models.Dto;
using airport.Models.Response;
using airport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace airport.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FlightController : ControllerBase
    {

        private readonly IFlightService _flightService;
        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        [Route("saludo")]
        public string Saludo()
        {
            return "Hola";
        }

        [HttpPost]
        [Authorize(Roles = "Encargado")]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] FlightDto flightDto)
        {
            var response = new Response();
            response.Success = false;
            try
            {
                var flight = await _flightService.createFlight(flightDto);
                response.Success = true;
                response.Message = "Vuelo creado correctamente";
                response.Data = flight;
            }
            catch (DbUpdateException ex)
            {
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Algo salió mal");
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("data-flight/{id}")]
        public async Task<IActionResult> DataFlight(int id)
        {
            var response = new Response();
            response.Success = false;
            try
            {
                var flight = await _flightService.findFlightById(id);
                response.Success = true;
                response.Message = "Datos del vuelo";
                response.Data = flight;
            }
            catch (ArgumentException ex)
            {
                return Ok(ex.Message);
            }
            catch (SqlException ex)
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
