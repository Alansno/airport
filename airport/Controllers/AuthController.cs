using airport.Models.Dto;
using airport.Models.Response;
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

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
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
            try
            {
                var isAuth = await _authService.authenticateUser(loginDto);
                response.Success = true;
                response.Message = "Usuario autenticado";
                response.Data = isAuth;
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
    }
}
