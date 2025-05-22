using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var authResult = await _identityService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);

            // Use authResult directly without conversion since the method signatures are now aligned
            if (!authResult.Success)
            {
                return BadRequest(new { Errors = authResult.Errors });
            }

            return Ok(authResult);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var authResult = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResult.Success)
            {
                return BadRequest(new { Errors = authResult.Errors });
            }

            return Ok(authResult);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResult = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResult.Success)
            {
                return BadRequest(new { Errors = authResult.Errors });
            }

            return Ok(authResult);
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}