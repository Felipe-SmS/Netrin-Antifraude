namespace Netrin.Antifraude.Application.Queries.Transacoes
{
    using MediatR;
    using Netrin.Antifraude.Application.Interfaces.Repositories;
    using Netrin.Antifraude.Application.Mapeamentos;
    using Netrin.Antifraude.Core.Dtos;

    public class ObterTransacaoQuery : IRequest<TransacaoDto?>
    {
        public Guid TransacaoId { get; set; }

        public class ObterTransacaoQueryHandler : IRequestHandler<ObterTransacaoQuery, TransacaoDto?>
        {
            private readonly ITransacaoRepository _transacaoRepository;

            public ObterTransacaoQueryHandler(ITransacaoRepository transacaoRepository)
            {
                _transacaoRepository = transacaoRepository;
            }

            public async Task<TransacaoDto?> Handle(ObterTransacaoQuery requisicao, CancellationToken cancelamento)
            {
                var transacao = await _transacaoRepository.ObterParaConsultaAsync(requisicao.TransacaoId, cancelamento);
                return transacao?.ParaDto();
            }
        }
    }
}
