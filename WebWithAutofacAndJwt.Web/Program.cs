using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebWithAutofacAndJwt.Entity;
using WebWithAutofacAndJwt.Web;
using WebWithAutofacAndJwt.Web.Extensions;
using WebWithAutofacAndJwt.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostContext, builder) =>
{
    // 使用用户机密
    if (hostContext.HostingEnvironment.IsDevelopment())
    {
        builder.AddUserSecrets<Program>();
    }
});

//配置Autofac为默认IOC容器
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(t => t.RegisterModule(new AutofacModule()));

var service = builder.Services;
//配置Json序列化
service.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    }
);

//获取配置
var jwtConfig = builder.Configuration.GetSection("jwtTokenConfig");
var jwtTokenConfig = jwtConfig.Get<JwtTokenConfig>();


//绑定配置
service.Configure<JwtTokenConfig>(jwtConfig);

//配置id生成器
service.RegisterIdGenService();

//添加数据库
service.AddDbContextPool<AppDbContext>(
        options =>
        options.UseNpgsql("Name=ConnectionStrings:PgSqlConnection", x => x.MigrationsAssembly("WebWithAutofacAndJwt.Migrations"))
    );
//添加Identity
service.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>()
    //�����������������������ơ����ĵ����ʼ��͸��ĵ绰��������Լ�˫���������֤�������ɵ�Ĭ�������ṩ���򡣣��ⶫ����ɶ����Ҳ�������
    .AddDefaultTokenProviders();

//添加jwt
service.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
        ValidAudience = jwtTokenConfig.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});


service.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebWithAutofac", Version = "v1" });

    //启用JWT认证
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT 鉴权",
        Description = "输入你的JWT Token（不需要加 ‘Bearer’）",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });

    #region swagger扫描xml
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var path = builder.Environment.ContentRootPath;
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    #endregion
});

//添加跨域
service.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebWithAutofac v1"));
}

app.UseCors();
app.UseHttpsRedirection();

//此处注意顺序不能反，而且必须在mapcontroller的上面。（理解中间件的原理）
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
