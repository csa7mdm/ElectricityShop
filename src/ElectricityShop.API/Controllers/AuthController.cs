using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityShop.API.Controllers
{
    /// <summary>
    /// Authentication controller for user registration, login, and token refresh operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="request">Registration details including email, password, first name, and last name</param>
        /// <returns>Authentication result with JWT token if successful</returns>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Registration failed due to validation errors</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _identityService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName);

            if (!result.Success)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and issues JWT token
        /// </summary>
        /// <param name="request">Login credentials - email and password</param>
        /// <returns>Authentication result with JWT token if successful</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Login failed due to invalid credentials</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(result);
        }

        /// <summary>
        /// Refreshes an expired JWT token using a refresh token
        /// </summary>
        /// <param name="request">The expired token and associated refresh token</param>
        /// <returns>New JWT token and refresh token if successful</returns>
        /// <response code="200">Token refresh successful</response>
        /// <response code="400">Token refresh failed</response>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!result.Success)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(result);
        }
    }

    /// <summary>
    /// Request model for user registration
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Email address of the user (must be unique)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password (must meet complexity requirements)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }
    }

    /// <summary>
    /// Request model for user login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Request model for refreshing JWT tokens
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// The expired JWT token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The refresh token issued with the original JWT
        /// </summary>
        public string RefreshToken { get; set; }
    }
}