using Autofac;
using Autofac.Extras.DynamicProxy;
using WebWithAutofacAndJwt.Web.Infrastructure;
using WebWithAutofacAndJwt.Web.Interceptor;
using WebWithAutofacAndJwt.Web.Service;

namespace WebWithAutofacAndJwt.Web;
public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        //注册service并启动接口代理
        builder.RegisterType<UserService>().As<IUserService>()
            .EnableInterfaceInterceptors();

        builder.RegisterType<JwtAuthManager>().As<IJwtAuthManager>();

        //注册拦截器
        builder.RegisterType<UserInterceptor>();
    }
}
