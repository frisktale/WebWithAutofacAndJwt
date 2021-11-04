namespace WebWithAutofacAndJwt.Web.Service
{
    public interface IUserService
    {
        Task<bool> IsAnExistingUserAsync(string userName);
        /// <summary>
        /// 校验用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>校验结果</returns>
        Task<bool> IsValidUserCredentialsAsync(string userName, string password);
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户权限</returns>
        Task<List<string>> GetUserRoleAsync(string userName);
        /// <summary>
        /// 注册时获取用户权限（初始化时使用）
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>权限</returns>
        Task<string> GetUserRoleOnRegisterAsync(string userName);
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="role">权限</param>
        /// <returns>是否成功</returns>
        Task<bool> RegisterAsync(string username, string password, string role);
        /// <summary>
        /// 添加权限至数据库
        /// </summary>
        /// <param name="role">权限名</param>
        /// <returns>是否成功</returns>
        Task<bool> AddRoleAsync(string role);
    }
}
