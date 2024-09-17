using AvoidLockApi;
using AvoidLockApi.UseCases.Lots;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AvoidLockDbContext>(
    opt => opt
        .UseNpgsql(builder.Configuration.GetConnectionString("AvoidLock"))
        .UseSnakeCaseNamingConvention());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ProcessBidsCommand>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();
