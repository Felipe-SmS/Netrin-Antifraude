namespace Netrin.Antifraude.Application.Commands.Transacoes
{
    using MediatR;
    using Netrin.Antifraude.Application.Domain.Interfaces;
    using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;
    using Netrin.Antifraude.Application.Mapeamentos;
    using Netrin.Antifraude.Core.Dtos;
    using Netrin.Antifraude.Application.Queries.Transacoes;
    using Netrin.Antifraude.Infrastructure.Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class AvaliarTransacaoCommand : IRequest<TransacaoDto?>
    {
        public int TransacaoId { get; set; }

        public class AvaliarTransacaoCommandHandler : IRequestHandler<AvaliarTransacaoCommand, TransacaoDto?>
        {
            private readonly ITransacaoRepository _transacaoRepository;
            private readonly IAvaliadorTransacao _avaliador;
            private readonly TransacaoQuery _query;
            private readonly AntifraudeDbContext _contexto;

            public AvaliarTransacaoCommandHandler(ITransacaoRepository transacaoRepository, IAvaliadorTransacao avaliador,
                TransacaoQuery query, AntifraudeDbContext contexto)
            {
                _transacaoRepository = transacaoRepository;
                _avaliador = avaliador;
                _query = query;
                _contexto = contexto;
            }

            public async Task<TransacaoDto?> Handle(AvaliarTransacaoCommand requisicao, CancellationToken cancelamento)
            {
                var transacao = await _query.ObterPeloIdAsync(requisicao.TransacaoId, cancelamento)
                    ?? throw new InvalidOperationException($"Transação {requisicao.TransacaoId} não encontrada.");
                if (transacao.Avaliacao is not null)
                    return transacao.ParaDto();

                var resultado = await _avaliador.AvaliarAsync(transacao, cancelamento);
                transacao.IniciarProcessamento();
                transacao.FinalizarAvaliacao(resultado.Decisao, resultado.Motivo);
                try
                {
                    await _transacaoRepository.AlterarAsync(transacao, cancelamento);
                }
                catch (DbUpdateException erro) when (erro.InnerException is PostgresException postgres &&
                    postgres.SqlState == PostgresErrorCodes.UniqueViolation &&
                    postgres.ConstraintName == "IX_Avaliacoes_TransacaoId")
                {
                    _contexto.ChangeTracker.Clear();
                    transacao = await _query.ObterPeloIdAsync(requisicao.TransacaoId, cancelamento)
                        ?? throw new InvalidOperationException($"Transação {requisicao.TransacaoId} não encontrada.");
                }
                return transacao.ParaDto();
            }
        }
    }
}

