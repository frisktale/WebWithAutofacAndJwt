using Autofac;
using Autofac.Extensions.DependencyInjection;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebWithAutofacAndJwt;
using WebWithAutofacAndJwt.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//配置Autofac为默认IOC容器
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(t => t.RegisterModule(new AutofacModule()));
// Add services to the container.

builder.Services.AddControllers(); 

//获取配置
var jwtConfig = builder.Configuration.GetSection("jwtTokenConfig");
var jwtTokenConfig = jwtConfig.Get<JwtTokenConfig>();
//绑定配置
builder.Services.Configure<JwtTokenConfig>(jwtConfig);

//添加jwt
builder.Services.AddAuthentication(x =>
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


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebWithAutofac", Version = "v1" });

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
});

//添加跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebWithAutofac v1"));
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

//此处注意顺序不能反，而且必须在mapcontroller的上面。（理解中间件的原理）
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
