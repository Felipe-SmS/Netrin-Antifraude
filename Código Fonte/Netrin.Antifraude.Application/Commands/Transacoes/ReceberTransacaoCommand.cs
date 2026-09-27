namespace Netrin.Antifraude.Application.Commands.Transacoes
{
    using MediatR;
    using Netrin.Antifraude.Application.Interfaces.Repositories;
    using Netrin.Antifraude.Application.Mapeamentos;
    using Netrin.Antifraude.Core.Dtos;

    public class ReceberTransacaoCommand : IRequest<TransacaoDto>
    {
        public ReceberNovaTransacao Transacao { get; set; } = new();

        public class ReceberTransacaoCommandHandler : IRequestHandler<ReceberTransacaoCommand, TransacaoDto>
        {
            private readonly ITransacaoRepository _transacaoRepository;

            public ReceberTransacaoCommandHandler(ITransacaoRepository transacaoRepository)
            {
                _transacaoRepository = transacaoRepository;
            }

            public async Task<TransacaoDto> Handle(ReceberTransacaoCommand requisicao, CancellationToken cancelamento)
            {
                var entrada = requisicao.Transacao;
                var existente = await _transacaoRepository.ObterPorChaveIdempotenciaAsync(
                    entrada.IdempotencyKey, cancelamento);
                if (existente is not null)
                    return existente.ParaDto();

                var transacao = new Transacao(entrada.IdempotencyKey, entrada.Valor);
                await _transacaoRepository.InserirAsync(transacao, cancelamento);
                return transacao.ParaDto();
            }
        }
    }
}

