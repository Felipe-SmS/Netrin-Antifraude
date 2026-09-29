
public class Transacao : BaseEntity
{
    public string IdempotencyKey { get; private set; } = null!;
    public decimal Valor { get; private set; }
    public bool EnvioParaAvaliacaoSolicitado { get; private set; }
    public StatusTransacao Status { get; private set; } = null!;
    public Avaliacao? Avaliacao { get; private set; }

    protected Transacao() { }

    public Transacao(string idempotencyKey, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("A chave de idempotência é obrigatória.");

        if (valor <= 0)
            throw new ArgumentException("O valor da transação deve ser maior que zero.");

        IdempotencyKey = idempotencyKey;
        Valor = valor;
        Status = new StatusTransacao(EnumStatusTransacao.Pendente);
    }

    public void IniciarProcessamento()
    {
        Status.AlterarStatus(EnumStatusTransacao.Processando);
    }

    public void SolicitarEnvioParaAvaliacao()
    {
        EnvioParaAvaliacaoSolicitado = true;
    }

    public void LiberarEnvioParaAvaliacao()
    {
        EnvioParaAvaliacaoSolicitado = false;
    }

    public void FinalizarAvaliacao(EnumDecisaoAvaliacao decisao, string? motivo)
    {
        Avaliacao = new Avaliacao(decisao, motivo);
        Status.AlterarStatus(EnumStatusTransacao.Processada);
    }

    public void RegistrarFalha()
    {
        Status.AlterarStatus(EnumStatusTransacao.Falha);
    }
}
