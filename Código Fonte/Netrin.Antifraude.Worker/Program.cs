using Netrin.Antifraude.Application.Commands.Transacoes;
using Netrin.Antifraude.Application.Avaliacao;
using Netrin.Antifraude.Worker;
using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Application.Interfaces.Repositories;
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
builder.Services.AddMediatR(configuracao =>
    configuracao.RegisterServicesFromAssemblyContaining<ReceberTransacaoCommand>());
builder.Services.AddScoped<IAvaliadorTransacao, AvaliadorRevisaoManual>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();



