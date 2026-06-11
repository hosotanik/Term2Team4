using Microsoft.Extensions.Options;
using ReservationServer.Repositries;
using System.Text.Json;
using Scalar.AspNetCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IReservationRepositry,ReservationRepositry>();

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {


     options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
 });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFetch", policy => 
        {
        policy
            .WithOrigins("http://127.0.0.1:5500"," http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowFetch");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
