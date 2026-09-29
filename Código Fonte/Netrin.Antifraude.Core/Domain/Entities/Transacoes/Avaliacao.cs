public class Avaliacao : BaseEntity
{
    public EnumDecisaoAvaliacao Decisao { get; private set; }
    public string? Motivo { get; private set; }

    protected Avaliacao() { }

    public Avaliacao(
        EnumDecisaoAvaliacao decisao,
        string? motivo)
    {
        Decisao = decisao;
        Motivo = motivo;
    }
}
