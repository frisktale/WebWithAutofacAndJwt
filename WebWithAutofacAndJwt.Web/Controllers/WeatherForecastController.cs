using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebWithAutofacAndJwt.Web.Infrastructure;
using WebWithAutofacAndJwt.Web.Model;
using WebWithAutofacAndJwt.Web.Service;
using static WebWithAutofacAndJwt.Web.Service.UserService;

namespace WebWithAutofacAndJwt.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IUserService userService,
            IJwtAuthManager jwtAuthManager
            )
        {
            _logger = logger;
            UserService = userService;
            _jwtAuthManager = jwtAuthManager;
        }

        public IUserService UserService { get; }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public IEnumerable<WeatherForecast> AdminGet()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.BasicUser)]
        public IEnumerable<WeatherForecast> BasicGet()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!UserService.IsValidUserCredentials(user.UserName, user.Password))
            {
                return Unauthorized();
            }

            var role = UserService.GetUserRole(user.UserName);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(claims, DateTime.Now);
            return Ok(jwtResult);
        }
    }
}