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
    // ʹ���û�����
    if (hostContext.HostingEnvironment.IsDevelopment())
    {
        builder.AddUserSecrets<Program>();
    }
});

//����AutofacΪĬ��IOC����
builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(t => t.RegisterModule(new AutofacModule()));

//����Json���л�
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    }
);

//��ȡ����
var jwtConfig = builder.Configuration.GetSection("jwtTokenConfig");
var jwtTokenConfig = jwtConfig.Get<JwtTokenConfig>();
//������
builder.Services.Configure<JwtTokenConfig>(jwtConfig);

//����id������
builder.Services.RegisterIdGenService();

//������ݿ�
builder.Services.AddDbContext<AppDbContext>(
        options => options.UseNpgsql("Name=ConnectionStrings:PgSqlConnection", x => x.MigrationsAssembly("WebWithAutofacAndJwt.Migrations")));
//���Identity
builder.Services.AddIdentity<User, IdentityRole<long>>().AddEntityFrameworkStores<AppDbContext>();

//���jwt
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

    //����JWT��֤
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT ��Ȩ",
        Description = "�������JWT Token������Ҫ�� ��Bearer����",
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

    #region swaggerɨ��xml
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var path = builder.Environment.ContentRootPath;
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    #endregion
});

//��ӿ���
builder.Services.AddCors(options =>
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

//�˴�ע��˳���ܷ������ұ�����mapcontroller�����档������м����ԭ��
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
