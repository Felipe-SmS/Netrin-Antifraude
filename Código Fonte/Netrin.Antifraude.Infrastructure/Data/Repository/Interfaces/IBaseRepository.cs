namespace Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces
{
    public enum FiltroStatus { Ativo, Inativo, Ambos }

    public interface IBaseReadRepository<TEntidade> where TEntidade : BaseEntity
    {
        Task<TEntidade> ObterPeloIdAsync(int id, CancellationToken cancelamento = default);
        Task<TEntidade> ObterPeloId(int id, CancellationToken cancelamento = default);
        IQueryable<TEntidade> Obter();
        IQueryable<TEntidade> Obter(FiltroStatus filtroStatus);
        Task<IEnumerable<TEntidade>> ObterListagem(CancellationToken cancelamento = default);
    }

    public interface IBaseWriteRepository<TEntidade> where TEntidade : BaseEntity
    {
        Task InserirAsync(TEntidade objeto, CancellationToken cancelamento = default);
        Task AlterarAsync(TEntidade objeto, CancellationToken cancelamento = default);
        Task AlterarVariosAsync(IEnumerable<TEntidade> objetos, CancellationToken cancelamento = default);
        Task RemoverAsync(int id, CancellationToken cancelamento = default);
        Task RemoverPorObjeto(TEntidade objeto, CancellationToken cancelamento = default);
    }

    public interface IBaseRepository<TEntidade> : IBaseReadRepository<TEntidade>, IBaseWriteRepository<TEntidade>
        where TEntidade : BaseEntity
    {
    }
}
