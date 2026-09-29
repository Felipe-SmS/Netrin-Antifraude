namespace Netrin.Antifraude.Application.Commands.Transacoes;

public class ConflitoIdempotenciaException : Exception
{
    public ConflitoIdempotenciaException()
        : base("A chave de idempotência já foi usada com outro valor.")
    {
    }
}
