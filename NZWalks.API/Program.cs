using Microsoft.EntityFrameworkCore;
using NZWalks.API;
using NZWalks.API.Mappings;
using NZWalks.API.Models;
using NZWalks.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext 주입, ConnectionString은 appsettings.json에서 설정함
var connectionString = builder.Configuration.GetConnectionString("NZWalksConnectionString");
builder.Services.AddDbContext<NZWalksDbContext>(options =>options.UseSqlServer(connectionString));

// Repository 주입
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();   //Region
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();   //Walk

// AutoMapper 주입
//Nuget 패키지 관리자에서 AutoMapper, AutoMapper DepedencyInjection 설치
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));   


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
