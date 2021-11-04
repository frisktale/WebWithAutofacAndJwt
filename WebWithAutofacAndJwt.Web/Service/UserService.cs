using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Identity;
using WebWithAutofacAndJwt.Entity;
using WebWithAutofacAndJwt.Web.Interceptor;
using WebWithAutofacAndJwt.Web.Model;
using Yitter.IdGenerator;

namespace WebWithAutofacAndJwt.Web.Service
{
    [Intercept(typeof(UserInterceptor))]
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        public UserService(ILogger<UserService> logger, UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 校验用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>校验结果</returns>
        public async Task<bool> IsValidUserCredentialsAsync(string userName, string password)
        {
            _logger.LogInformation("Validating user [{userName}]", userName);
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(existingUser, password);

        }

        public async Task<bool> IsAnExistingUserAsync(string userName)
        {
            return (await _userManager.FindByNameAsync(userName)) == null;
        }

        public async Task<List<string>> GetUserRoleAsync(string userName)
        {

            var roles = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userName));
            return roles?.ToList() ?? new List<string>();
        }
        public async Task<string> GetUserRoleOnRegisterAsync(string userName)
        {

            string role;
            if (userName == "admin")
            {
                role = UserRoles.Admin;
            }
            else
            {
                role = UserRoles.BasicUser;
            }
            return await Task.FromResult(role);
        }

        public async Task<bool> RegisterAsync(string userName, string password, string role)
        {

            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                return false;
            }

            User user = new() { Id = YitIdHelper.NextId(), UserName = userName, SecurityStamp = Guid.NewGuid().ToString() };
            var res = await _userManager.CreateAsync(user, password);
            var roleRes = await _userManager.AddToRoleAsync(user, role);
            return res.Succeeded && roleRes.Succeeded;
        }

        public async Task<bool> AddRoleAsync(string role)
        {
            var res = await _roleManager.CreateAsync(new IdentityRole<long> { Name = role });
            return res.Succeeded;
        }
    }
}
