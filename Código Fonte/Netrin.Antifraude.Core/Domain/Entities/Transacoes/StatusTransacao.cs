public class StatusTransacao : BaseEntity
{
    public EnumStatusTransacao Status { get; private set; }

    protected StatusTransacao() { }

    public StatusTransacao(EnumStatusTransacao status)
    {
        Status = status;
    }

    public void AlterarStatus(EnumStatusTransacao status)
    {
        Status = status;
    }
}
