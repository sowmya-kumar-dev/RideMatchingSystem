using Microsoft.EntityFrameworkCore;
using RideMatchingSystem.api.Data;
using RideMatchingSystem.api.Services;
using RideMatchingSystem.api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RideMatchingService>();
builder.Services.AddHostedService<RideMatchingWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();



app.MapHub<RideHub>("/rideHub");

app.Run();