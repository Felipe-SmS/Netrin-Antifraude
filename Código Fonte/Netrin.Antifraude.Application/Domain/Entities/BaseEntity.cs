public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; protected set; }
    public bool EhAtivo {get; set;} = true;

    protected void AtualizarData()
    {
        DataAtualizacao = DateTime.UtcNow;
    }
}