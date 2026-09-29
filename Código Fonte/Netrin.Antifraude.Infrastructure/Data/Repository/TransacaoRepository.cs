using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;
using Netrin.Antifraude.Infrastructure.Data.Context;

namespace Netrin.Antifraude.Infrastructure.Repositories;

public class TransacaoRepository : BaseRepository<Transacao, AntifraudeDbContext>, ITransacaoRepository
{
    public TransacaoRepository(AntifraudeDbContext contexto) : base(contexto)
    {
    }

    public override Task AlterarAsync(Transacao transacao, CancellationToken cancelamento = default)
    {
        if (transacao.Avaliacao is not null && Contexto.Entry(transacao.Avaliacao).State == EntityState.Detached)
            Contexto.Avaliacoes.Add(transacao.Avaliacao);

        return base.AlterarAsync(transacao, cancelamento);
    }
}
