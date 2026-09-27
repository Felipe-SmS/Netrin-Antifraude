using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Application.Interfaces.Repositories;
using Netrin.Antifraude.Infrastructure.Data.Context;

namespace Netrin.Antifraude.Infrastructure.Repositories;

public class TransacaoRepository : BaseRepository<Transacao, AntifraudeDbContext>, ITransacaoRepository
{
    public TransacaoRepository(AntifraudeDbContext contexto) : base(contexto)
    {
    }

    protected override IQueryable<Transacao> CriarConsulta() =>
        Contexto.Set<Transacao>().Include(transacao => transacao.Status)
            .Include(transacao => transacao.Avaliacao);

    public Task<Transacao?> ObterParaConsultaAsync(Guid id,
        CancellationToken cancelamento = default) =>
        CriarConsulta().AsNoTracking().SingleOrDefaultAsync(transacao => transacao.Id == id, cancelamento);

    public Task<Transacao?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia,
        CancellationToken cancelamento = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chaveIdempotencia);
        return CriarConsulta().SingleOrDefaultAsync(
            transacao => transacao.IdempotencyKey == chaveIdempotencia, cancelamento);
    }
}

