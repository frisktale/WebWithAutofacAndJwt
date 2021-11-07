using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebWithAutofacAndJwt.Web.Infrastructure;
using WebWithAutofacAndJwt.Web.Model;
using WebWithAutofacAndJwt.Web.Service;

namespace WebWithAutofacAndJwt.Web.Controllers
{
    /// <summary>
    /// �û�������
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<UserController> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IJwtAuthManager jwtAuthManager
        ) => (_logger, _userService, _jwtAuthManager) = (logger, userService, jwtAuthManager);


        /// <summary>
        /// ���Թ���ԱȨ��
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// ������ͨ�û�Ȩ��
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// �û���¼
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(string userName, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!await _userService.IsValidUserCredentialsAsync(userName, password))
            {
                return Unauthorized();
            }

            var role = await _userService.GetUserRoleAsync(userName);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, string.Join(',',role))
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(claims, DateTime.Now);
            return Ok(jwtResult);
        }

        /// <summary>
        /// ע���û�
        /// </summary>
        /// <param name="user">�û�model</param>
        /// <returns>�Ƿ�ɹ�</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var role = await _userService.GetUserRoleOnRegisterAsync(user.UserName);
            var res = await _userService.RegisterAsync(user.UserName, user.Password, role);
            return Ok(res);
        }

        /// <summary>
        /// ��ʼ���û�Ȩ�ޣ������ã�
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> InitRoleAsync()
        {
            var adminRes = await _userService.AddRoleAsync(UserRoles.Admin);
            var basicRes = await _userService.AddRoleAsync(UserRoles.BasicUser);
            return Ok(adminRes && basicRes);
        }
    }
}