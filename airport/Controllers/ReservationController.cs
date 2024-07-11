using airport.Models.Dto;
using airport.Models.Response;
using airport.Services.Interfaces;
using airport.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace airport.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] ReservationDto reservationDto)
        {
            var response = new Response();
            response.Success = false;
            try
            {
                var userId = Utilities.findUserId(HttpContext);
                var reservation = await _reservationService.createReservation(reservationDto, userId);
                response.Success = true;
                response.Message = "Reservación creada correctamente";

            }
            catch (ArgumentException ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
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

        [HttpGet]
        [Route("get-by-user")]
        public async Task<IActionResult> getByUser()
        {
            var response = new Response();
            response.Success = false;
            try
            {
                var userId = Utilities.findUserId(HttpContext);
                var reservationUser = await _reservationService.getReservationsByUser(userId);
                response.Success = true;
                response.Data = reservationUser;
                response.Message = "Reservaciones del usuario";
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("delete-reservation-soft/{id}")]
        public async Task<IActionResult> deleteReservationSoft([FromRoute] int id)
        {
            var response = new Response();
            response.Success = false;

            try
            {
                var userId = Utilities.findUserId(HttpContext);
                var deleting = await _reservationService.cancelReservationSoft(id, userId);
                response.Success = true;
                response.Message = "Reservación cancelada suavemente";
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-reservation/{id}")]
        public async Task<IActionResult> deleteReservation([FromRoute] int id)
        {
            var response = new Response();
            response.Success = false;

            try
            {
                var userId = Utilities.findUserId(HttpContext);
                var deleting = await _reservationService.cancelReservationPer(id, userId);
                response.Success = true;
                response.Message = "Reservación cancelada permanentemente";
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(response);
        }
    }
}
