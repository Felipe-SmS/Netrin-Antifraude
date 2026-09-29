using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Infrastructure.Data.Context;

namespace Netrin.Antifraude.Application.Queries.Transacoes;

public class TransacaoQuery
{
    private readonly AntifraudeDbContext _contexto;

    public TransacaoQuery(AntifraudeDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Transacao?> ObterPorChaveIdempotenciaAsync(string chave, CancellationToken cancelamento = default)
    {
        return Consultar().SingleOrDefaultAsync(transacao => transacao.IdempotencyKey == chave, cancelamento);
    }

    public Task<Transacao?> ObterPeloIdAsync(int id, CancellationToken cancelamento = default)
    {
        return Consultar().SingleOrDefaultAsync(transacao => transacao.Id == id, cancelamento);
    }

    private IQueryable<Transacao> Consultar()
    {
        return _contexto.Transacoes
            .Include(transacao => transacao.Status)
            .Include(transacao => transacao.Avaliacao);
    }
}
