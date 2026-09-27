namespace Netrin.Antifraude.Application.Interfaces.Repositories;

public interface ITransacaoRepository : IBaseRepository<Transacao>
{
    Task<Transacao?> ObterParaConsultaAsync(Guid id, CancellationToken cancelamento = default);
    Task<Transacao?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia,
        CancellationToken cancelamento = default);
}
