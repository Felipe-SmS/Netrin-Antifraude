namespace Netrin.Antifraude.Core.Dtos;

public class TransacaoDto
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public StatusTransacaoDto Status { get; set; } = new();
    public AvaliacaoDto? Avaliacao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool EhAtivo { get; set; }
}

public class StatusTransacaoDto
{
    public Guid Id { get; set; }
    public EnumStatusTransacao Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool EhAtivo { get; set; }
}

public class AvaliacaoDto
{
    public Guid Id { get; set; }
    public EnumDecisaoAvaliacao Decisao { get; set; }
    public string? Motivo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool EhAtivo { get; set; }
}
