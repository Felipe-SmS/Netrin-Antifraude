using Netrin.Antifraude.Application.Commands.Transacoes;
using Netrin.Antifraude.Application.Avaliacao;
using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Application.Interfaces.Repositories;
using Netrin.Antifraude.Infrastructure.Repositories;
using Netrin.Antifraude.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings:DefaultConnection para acessar o PostgreSQL.");
}

builder.Services.AddDbContext<AntifraudeDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddMediatR(configuracao =>
    configuracao.RegisterServicesFromAssemblyContaining<ReceberTransacaoCommand>());
builder.Services.AddScoped<IAvaliadorTransacao, AvaliadorRevisaoManual>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



