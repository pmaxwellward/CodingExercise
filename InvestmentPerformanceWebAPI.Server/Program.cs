using InvestmentPerformanceWebAPI.Server.Database.Context;
using InvestmentPerformanceWebAPI.Server.Database.Repository;
using InvestmentPerformanceWebAPI.Server.Services;
using Microsoft.EntityFrameworkCore;

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


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
