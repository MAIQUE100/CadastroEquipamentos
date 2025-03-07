using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using EquipmentManagementWebApp.Server.Domain.Interfaces;
using EquipmentManagementWebApp.Server.Application.Services;
using EquipmentManagementWebApp.Server.Application.Mappers;
using EquipmentCommon.CommonInterfaces.Repositories;
using EquipmentCommon.CommonInterfaces.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
    policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    //options.AddPolicy("AllowSpecificOrigin",
    //    policy =>
    //    {
    //        policy.WithOrigins("http://localhost:52866")
    //        .AllowAnyMethod()
    //        .AllowAnyHeader();
    //    });
    //options.AddPolicy("AllowSpecificOrigin",
    //    policy =>
    //    {
    //        policy.WithOrigins("https://127.0.0.1:52866/")
    //        .AllowAnyMethod()
    //        .AllowAnyHeader();
    //    });
});



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddAutoMapper(typeof(EquipmentViewModelProfile));

var app = builder.Build();
app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
