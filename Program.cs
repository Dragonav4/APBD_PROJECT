using APBD_PROJECT.Data;
using APBD_PROJECT.Interfaces;
using APBD_PROJECT.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultSqLite");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
  dbContext.Database.Migrate();
}
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();