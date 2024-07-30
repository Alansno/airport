using airport.Models.Dto;
using airport.Models.Response;
using airport.Services;
using airport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace airport.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = new Response();
            response.Success = false;
            try
            {
                var user = await _authService.registerUser(registerDto);
                response.Success = true;
                response.Message = "Usuario registrado correctamente";
                response.Data = user;
            }
            catch (ArgumentException ex)
            {
                response.Message = ex.Message;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] LoginDto loginDto)
        {
            var response = new Response();
            response.Success = false;
                var isAuth = await _authService.AuthenticateUser(loginDto);
                if (isAuth.IsSuccess)
                {
                    response.Success = true;
                    response.Message = "Usuario autenticado";
                    response.Data = isAuth.Value;
                    return Ok(response);
                }

                return BadRequest(isAuth.Error);            
        }
    }
}
