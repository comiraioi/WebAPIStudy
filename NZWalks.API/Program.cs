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

// DbContext ����, ConnectionString�� appsettings.json���� ������
var connectionString = builder.Configuration.GetConnectionString("NZWalksConnectionString");
builder.Services.AddDbContext<NZWalksDbContext>(options =>options.UseSqlServer(connectionString));

// Repository ����
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();   //Region
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();   //Walk

// AutoMapper ����
//Nuget ��Ű�� �����ڿ��� AutoMapper, AutoMapper DepedencyInjection ��ġ
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
