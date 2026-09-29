namespace Netrin.Antifraude.Application.Commands.Transacoes
{
    using MediatR;
    using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;
    using Netrin.Antifraude.Application.Mapeamentos;
    using Netrin.Antifraude.Core.Dtos;
    using Netrin.Antifraude.Core.RabbitMq.Domain.Entities;
    using Netrin.Antifraude.Application.RabbitMQ;
    using Netrin.Antifraude.Application.Queries.Transacoes;
    using Netrin.Antifraude.Infrastructure.Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class ReceberTransacaoCommand : IRequest<TransacaoDto>
    {
        public ReceberNovaTransacao Transacao { get; set; } = new();

        public class ReceberTransacaoCommandHandler : IRequestHandler<ReceberTransacaoCommand, TransacaoDto>
        {
            private readonly ITransacaoRepository _transacaoRepository;
            private readonly Publisher _publisher;
            private readonly TransacaoQuery _query;
            private readonly AntifraudeDbContext _contexto;

            public ReceberTransacaoCommandHandler(ITransacaoRepository transacaoRepository, Publisher publisher,
                TransacaoQuery query, AntifraudeDbContext contexto)
            {
                _transacaoRepository = transacaoRepository;
                _publisher = publisher;
                _query = query;
                _contexto = contexto;
            }

            public async Task<TransacaoDto> Handle(ReceberTransacaoCommand requisicao, CancellationToken cancelamento)
            {
                var entrada = requisicao.Transacao;
                var existente = await _query.ObterPorChaveIdempotenciaAsync(entrada.IdempotencyKey, cancelamento);
                if (existente is not null)
                {
                    ValidarValor(existente, entrada.Valor);
                    await PublicarSeNecessarioAsync(existente, cancelamento);
                    return existente.ParaDto();
                }

                var transacao = new Transacao(entrada.IdempotencyKey, entrada.Valor);
                try
                {
                    await _transacaoRepository.InserirAsync(transacao, cancelamento);
                }
                catch (DbUpdateException erro) when (erro.InnerException is PostgresException postgres &&
                    postgres.SqlState == PostgresErrorCodes.UniqueViolation &&
                    postgres.ConstraintName == "IX_Transacoes_IdempotencyKey")
                {
                    _contexto.ChangeTracker.Clear();
                    transacao = await _query.ObterPorChaveIdempotenciaAsync(entrada.IdempotencyKey, cancelamento)
                        ?? throw new InvalidOperationException("A transação já existe, mas não pôde ser consultada.");
                }
                ValidarValor(transacao, entrada.Valor);
                await PublicarSeNecessarioAsync(transacao, cancelamento);
                return transacao.ParaDto();
            }

            private async Task PublicarSeNecessarioAsync(Transacao transacao, CancellationToken cancelamento)
            {
                for (var tentativa = 0; tentativa < 3; tentativa++)
                {
                    if (transacao.Avaliacao is not null || transacao.EnvioParaAvaliacaoSolicitado)
                        return;

                    transacao.SolicitarEnvioParaAvaliacao();
                    try
                    {
                        await _transacaoRepository.AlterarAsync(transacao, cancelamento);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _contexto.ChangeTracker.Clear();
                        transacao = await _query.ObterPeloIdAsync(transacao.Id, cancelamento)
                            ?? throw new InvalidOperationException("A transação não pôde ser consultada.");
                        continue;
                    }

                    try
                    {
                        await _publisher.PublishAsync(new TransacaoRecebida(transacao.Id), cancelamento);
                    }
                    catch
                    {
                        transacao.LiberarEnvioParaAvaliacao();
                        await _transacaoRepository.AlterarAsync(transacao, CancellationToken.None);
                        throw;
                    }

                    return;
                }

                throw new InvalidOperationException("Não foi possível reservar o envio da transação para avaliação.");
            }

            private static void ValidarValor(Transacao transacao, decimal valor)
            {
                if (transacao.Valor != valor)
                    throw new ConflitoIdempotenciaException();
            }
        }
    }
}

