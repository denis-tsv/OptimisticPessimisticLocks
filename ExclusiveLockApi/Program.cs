using ExclusiveLockApi;
using ExclusiveLockApi.Images;
using ExclusiveLockApi.Services;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ExclusiveLockDbContext>(
    opt => opt
        .UseNpgsql(builder.Configuration.GetConnectionString("ExclusiveLock"))
        .UseSnakeCaseNamingConvention());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<StartRenderImageCommand>());
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
LinqToDBForEFTools.Initialize();

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
