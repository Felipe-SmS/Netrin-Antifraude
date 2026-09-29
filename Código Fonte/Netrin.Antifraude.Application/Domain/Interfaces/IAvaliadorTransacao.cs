namespace Netrin.Antifraude.Application.Domain.Interfaces;

public interface IAvaliadorTransacao
{
    Task<Avaliacao> AvaliarAsync(Transacao transacao, CancellationToken cancelamento = default);
}
