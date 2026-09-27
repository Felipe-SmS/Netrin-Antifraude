namespace Netrin.Antifraude.Core.Dtos;

public class ReceberNovaTransacao
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
