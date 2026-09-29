namespace Netrin.Antifraude.Application.Domain.Builders;

public sealed class AvaliacaoBuilder
{
    private readonly List<string> _rejeicoes = [];
    private readonly List<string> _revisoes = [];

    public AvaliacaoBuilder RejeitarSe(bool condicao, string motivo)
    {
        if (condicao)
            _rejeicoes.Add(motivo);
        return this;
    }

    public AvaliacaoBuilder RevisarSe(bool condicao, string motivo)
    {
        if (condicao)
            _revisoes.Add(motivo);
        return this;
    }

    public Avaliacao Construir()
    {
        if (_rejeicoes.Count > 0)
            return new Avaliacao(EnumDecisaoAvaliacao.Rejeitada, string.Join("; ", _rejeicoes));

        if (_revisoes.Count > 0)
            return new Avaliacao(EnumDecisaoAvaliacao.RevisaoManual, string.Join("; ", _revisoes));

        return new Avaliacao(EnumDecisaoAvaliacao.Aprovada, "Nenhuma regra de risco identificada.");
    }
}
