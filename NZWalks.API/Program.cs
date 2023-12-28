using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks.API;
using NZWalks.API.Mappings;
using NZWalks.API.Models;
using NZWalks.API.Repositories;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 서버 시간 기준 하루마다 로그 파일 생성
var logger = new LoggerConfiguration().WriteTo.Console()
                                    .WriteTo.File("Logs/NzWalks_Log.txt", rollingInterval: RollingInterval.Day)
                                    .MinimumLevel.Information().CreateLogger();

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();  // File Path 설정

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "NZ Walks API", Version = "v1"});

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In= ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// DbContext 주입, ConnectionString은 appsettings.json에서 설정함
var connectionString = builder.Configuration.GetConnectionString("NZWalksConnectionString");
builder.Services.AddDbContext<NZWalksDbContext>(options =>options.UseSqlServer(connectionString));

var authConnectionString = builder.Configuration.GetConnectionString("NZWalksAuthConnectionString");
builder.Services.AddDbContext<NZWalksAuthDbContext>(options => options.UseSqlServer(authConnectionString));


// Repository 주입
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();   //Region
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();       //Walk
builder.Services.AddScoped<ITokenRepository, TokenRepository>();        //Token
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();   //Image

// AutoMapper 주입
//Nuget 패키지 관리자에서 AutoMapper, AutoMapper DepedencyInjection 설치
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));   


// 아이덴티티 주입
builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
                .AddEntityFrameworkStores<NZWalksAuthDbContext>()
                .AddDefaultTokenProviders();

// 비밀번호 형식 지정
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;        // 6자리 이상
    options.Password.RequiredUniqueChars = 1;   // 특수기호 1개 이상
});

// JWT 인증 주입
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<ExceptionHandlerMiddleware>();    // 예외 처리 미들웨어

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// https://localhost:7202/api/Images 경로를 실제 파일이 있는 로컬 경로로 매핑
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});

app.MapControllers();

app.Run();
