using Netrin.Antifraude.Core.Dtos;

namespace Netrin.Antifraude.Application.Mapeamentos;

public static class TransacaoMapeamento
{
    public static TransacaoDto ParaDto(this Transacao transacao) => new()
    {
        Id = transacao.Id,
        IdempotencyKey = transacao.IdempotencyKey,
        Valor = transacao.Valor,
        DataCriacao = transacao.DataCriacao,
        DataAtualizacao = transacao.DataAtualizacao,
        EhAtivo = transacao.EhAtivo,
        Status = new StatusTransacaoDto
        {
            Id = transacao.Status.Id,
            Status = transacao.Status.Status,
            StatusDescricao = transacao.Status.Status.ToString(),
            DataCriacao = transacao.Status.DataCriacao,
            DataAtualizacao = transacao.Status.DataAtualizacao,
            EhAtivo = transacao.Status.EhAtivo
        },
        Avaliacao = transacao.Avaliacao is null ? null : new AvaliacaoDto
        {
            Id = transacao.Avaliacao.Id,
            Decisao = transacao.Avaliacao.Decisao,
            DecisaoDescricao = transacao.Avaliacao.Decisao.ToString(),
            Motivo = transacao.Avaliacao.Motivo,
            DataCriacao = transacao.Avaliacao.DataCriacao,
            DataAtualizacao = transacao.Avaliacao.DataAtualizacao,
            EhAtivo = transacao.Avaliacao.EhAtivo
        }
    };
}
