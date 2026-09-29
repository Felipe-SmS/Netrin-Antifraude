using Netrin.Antifraude.Application.Commands.Transacoes;
using Netrin.Antifraude.Application.Domain.Interfaces;
using Netrin.Antifraude.Application.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;
using Netrin.Antifraude.Infrastructure.Repositories;
using Netrin.Antifraude.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Netrin.Antifraude.Api.Autenticacao;
using MassTransit;
using Netrin.Antifraude.Application.RabbitMQ;
using Netrin.Antifraude.Application.Queries.Transacoes;
using Microsoft.OpenApi;

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
builder.Services.AddScoped<TransacaoQuery>();
builder.Services.AddMediatR(configuracao =>
    configuracao.RegisterServicesFromAssemblyContaining<ReceberTransacaoCommand>());
builder.Services.AddScoped<IAvaliadorTransacao, AvaliadorTransacao>();
builder.Services.AddScoped<Publisher>();
builder.Services.AddMassTransit(o =>
{
    o.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["HostNameRabbit"], h =>
        {
            h.Username(builder.Configuration["UserNameRabbit"]!);
            h.Password(builder.Configuration["PasswordRabbit"]!);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.SwaggerDoc("v1", new OpenApiInfo { Title = "Netrin Antifraude API", Version = "v1" });
    opcoes.AddSecurityDefinition(BasicAuthHandler.Esquema, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic"
    });
    opcoes.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(BasicAuthHandler.Esquema, documento)] = []
    });
});
builder.Services.AddAuthentication(BasicAuthHandler.Esquema)
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(BasicAuthHandler.Esquema, _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AntifraudeDbContext>();
    await contexto.Database.MigrateAsync();
}

if (builder.Configuration.GetValue<bool>("MigrateOnly"))
{
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "Netrin Antifraude API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
