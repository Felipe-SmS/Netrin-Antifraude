namespace Netrin.Antifraude.Application.Commands.Transacoes
{
    using MediatR;
    using Netrin.Antifraude.Application.Avaliacao;
    using Netrin.Antifraude.Application.Interfaces.Repositories;
    using Netrin.Antifraude.Application.Mapeamentos;
    using Netrin.Antifraude.Core.Dtos;

    public class AvaliarTransacaoCommand : IRequest<TransacaoDto?>
    {
        public Guid TransacaoId { get; set; }

        public class AvaliarTransacaoCommandHandler : IRequestHandler<AvaliarTransacaoCommand, TransacaoDto?>
        {
            private readonly ITransacaoRepository _transacaoRepository;
            private readonly IAvaliadorTransacao _avaliador;

            public AvaliarTransacaoCommandHandler(ITransacaoRepository transacaoRepository, IAvaliadorTransacao avaliador)
            {
                _transacaoRepository = transacaoRepository;
                _avaliador = avaliador;
            }

            public async Task<TransacaoDto?> Handle(AvaliarTransacaoCommand requisicao, CancellationToken cancelamento)
            {
                var transacao = await _transacaoRepository.ObterPeloIdAsync(requisicao.TransacaoId, cancelamento);
                if (transacao.Avaliacao is not null)
                    return transacao.ParaDto();

                var resultado = await _avaliador.AvaliarAsync(transacao, cancelamento);
                transacao.IniciarProcessamento();
                transacao.FinalizarAvaliacao(resultado.Decisao, resultado.Motivo);
                await _transacaoRepository.AlterarAsync(transacao, cancelamento);
                return transacao.ParaDto();
            }
        }
    }
}

