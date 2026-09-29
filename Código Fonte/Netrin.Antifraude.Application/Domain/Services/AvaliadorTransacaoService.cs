using Netrin.Antifraude.Application.Domain.Builders;
using Netrin.Antifraude.Application.Domain.Interfaces;

namespace Netrin.Antifraude.Application.Domain.Services;

public sealed class AvaliadorTransacao : IAvaliadorTransacao
{
    private const decimal ValorMaximo = 50_000m;
    private const decimal ValorParaRevisao = 10_000m;
    private const decimal ValorMinimoSemRevisao = 1m;

    public Task<Avaliacao> AvaliarAsync(Transacao transacao, CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(transacao);
        cancelamento.ThrowIfCancellationRequested();

        var avaliacao = new AvaliacaoBuilder()
            .RejeitarSe(!transacao.EhAtivo, "Transação inativa.")
            .RejeitarSe(transacao.Valor > ValorMaximo, "Valor acima do limite de R$ 50.000.")
            .RejeitarSe(decimal.Round(transacao.Valor, 2) != transacao.Valor,
                "Valor com fração de centavo.")
            .RevisarSe(transacao.Valor >= ValorParaRevisao, "Valor a partir de R$ 10.000 exige revisão manual.")
            .RevisarSe(transacao.Valor < ValorMinimoSemRevisao, "Valor abaixo de R$ 1 exige revisão manual.")
            .Construir();

        return Task.FromResult(avaliacao);
    }
}
