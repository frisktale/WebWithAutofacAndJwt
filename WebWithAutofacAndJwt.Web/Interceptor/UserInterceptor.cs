using Castle.DynamicProxy;

namespace WebWithAutofacAndJwt.Web.Interceptor;

public class UserInterceptor : IInterceptor
{
    private readonly ILogger<UserInterceptor> _logger;

    public UserInterceptor(ILogger<UserInterceptor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// AOP示例
    /// </summary>
    /// <param name="invocation">待执行的方法</param>
    public void Intercept(IInvocation invocation)
    {

        var name = invocation.Method.Name;

        _logger.LogInformation("{name}运行前", name);
        invocation.Proceed();
        _logger.LogInformation("{name}运行后", name);
    }
}

