using Netrin.Antifraude.Application.Commands.Transacoes;
using Netrin.Antifraude.Application.Domain.Interfaces;
using Netrin.Antifraude.Application.Domain.Services;
using MassTransit;
using Netrin.Antifraude.Application.RabbitMQ;
using Netrin.Antifraude.Application.Queries.Transacoes;
using Netrin.Antifraude.Worker.RabbitMQs.Consumer;
using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;
using Netrin.Antifraude.Infrastructure.Repositories;
using Netrin.Antifraude.Infrastructure.Data.Context;

var builder = Host.CreateApplicationBuilder(args);
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
builder.Services.AddScoped<TransacaoQuery>();
builder.Services.AddMediatR(configuracao =>
    configuracao.RegisterServicesFromAssemblyContaining<ReceberTransacaoCommand>());
builder.Services.AddScoped<IAvaliadorTransacao, AvaliadorTransacao>();
builder.Services.AddScoped<Publisher>();
builder.Services.AddMassTransit(o =>
{
    o.SetKebabCaseEndpointNameFormatter();
    o.AddConsumer<AvaliarTransacaoConsumer>();

    o.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["HostNameRabbit"], h =>
        {
            h.Username(builder.Configuration["UserNameRabbit"]!);
            h.Password(builder.Configuration["PasswordRabbit"]!);
        });

        cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.PrefetchCount = 1;
        cfg.ConfigureEndpoints(context,
            new KebabCaseEndpointNameFormatter(builder.Configuration["EnvironmentRabbit"], false));
    });
});

var host = builder.Build();
host.Run();
