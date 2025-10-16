using InvestmentPerformanceWebAPI.Server.Database.Context;
using InvestmentPerformanceWebAPI.Server.Database.Repository;
using InvestmentPerformanceWebAPI.Server.Services;
using Microsoft.EntityFrameworkCore;
using InvestmentPerformanceWebAPI.Server.Middleware;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // quieter framework noise
    .Enrich.FromLogContext()
    .WriteTo.Console() // dev: console only (no files) demo purposes only
                    
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string cs = builder.Configuration.GetConnectionString("Sqlite")
    ?? "Data Source=investment.db";

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlite(cs);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddScoped<IInvestmentQueryService, InvestmentQueryService>();
builder.Services.AddSingleton<IPriceService, InMemoryPriceService>();

builder.Host.UseSerilog((ctx, cfg) => {
    cfg.MinimumLevel.Information()
       .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
       .Enrich.FromLogContext()
       .WriteTo.Console();
    // If you want to toggle log level at runtime, use a LoggingLevelSwitch
});


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.UseGlobalExceptionHandler(app.Environment, logger);

// strictly for demo purposes
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSerilogRequestLogging(); // logs minimal per-request info

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
