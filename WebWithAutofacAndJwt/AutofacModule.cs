using Autofac;
using Autofac.Extras.DynamicProxy;
using WebWithAutofacAndJwt.Infrastructure;
using WebWithAutofacAndJwt.Interceptor;
using WebWithAutofacAndJwt.Service;

namespace WebWithAutofacAndJwt;
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

