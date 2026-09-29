public abstract class BaseEntity
{
    public int Id { get; protected set; } = 0;

    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; protected set; }
    public bool EhAtivo { get; set; } = true;

    protected void AtualizarData()
    {
        DataAtualizacao = DateTime.UtcNow;
    }
}
