using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims; // Required for ClaimsPrincipal

namespace WeatherForecast.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This endpoint is publicly accessible and requires no authentication.
        /// </summary>
        /// <returns>A simple public message.</returns>
        [HttpGet("public")]
        [AllowAnonymous] // Explicitly allows anonymous access, even if controller had [Authorize]
        public IActionResult GetPublicMessage()
        {
            _logger.LogInformation("Accessing public endpoint.");
            return Ok("This is a public message. No authentication required.");
        }

        /// <summary>
        /// This endpoint requires any authenticated user.
        /// </summary>
        /// <returns>A message indicating successful authentication.</returns>
        [HttpGet("authenticated")]
        [Authorize] // Requires any valid authenticated user
        public IActionResult GetAuthenticatedMessage()
        {
            _logger.LogInformation("Accessing authenticated endpoint.");
            // You can access user information here, e.g., User.Identity.Name
            return Ok($"Hello, {User.Identity?.Name ?? "Authenticated User"}! You are authenticated.");
        }

        /// <summary>
        /// This endpoint requires an authenticated user with the 'RequiresForecastReadScope' policy.
        /// This policy is defined in Program.cs to require the 'forecast.read' scope.
        /// </summary>
        /// <returns>A message indicating successful authorization with the required scope.</returns>
        [HttpGet("scoped-read")]
        [Authorize(Policy = "location.locations")] // Requires the 'forecast.read' scope
        public IActionResult GetScopedReadMessage()
        {
            _logger.LogInformation("Accessing scoped-read endpoint.");
            return Ok($"You have successfully accessed the scoped-read endpoint. Your token has 'forecast.read' scope.");
        }

        /// <summary>
        /// This endpoint requires an authenticated user and displays all claims from their token.
        /// Useful for debugging and understanding what claims are present.
        /// </summary>
        /// <returns>A dictionary of claims.</returns>
        [HttpGet("claims")]
        [Authorize] // Requires any valid authenticated user
        public IActionResult GetClaims()
        {
            _logger.LogInformation("Accessing claims endpoint.");
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}