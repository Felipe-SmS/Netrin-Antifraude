using Microsoft.EntityFrameworkCore;
using Netrin.Antifraude.Infrastructure.Data.Repository.Interfaces;

namespace Netrin.Antifraude.Infrastructure.Repositories
{
    public interface IBaseRepository<TEntidade, TContexto> : IBaseRepository<TEntidade>
        where TEntidade : BaseEntity where TContexto : DbContext
    {
    }

    public class BaseRepository<TEntidade, TContexto> : IBaseRepository<TEntidade, TContexto>
        where TEntidade : BaseEntity where TContexto : DbContext
    {
        private readonly TContexto _contexto;
        protected TContexto Contexto => _contexto;

        public BaseRepository(TContexto contexto)
        {
            _contexto = contexto;
        }

        protected virtual IQueryable<TEntidade> CriarConsulta() => Contexto.Set<TEntidade>();

        public IQueryable<TEntidade> Obter() => Obter(FiltroStatus.Ativo);

        public IQueryable<TEntidade> Obter(FiltroStatus filtroStatus)
        {
            return filtroStatus switch
            {
                FiltroStatus.Ativo => CriarConsulta().Where(objeto => objeto.EhAtivo),
                FiltroStatus.Inativo => CriarConsulta().Where(objeto => !objeto.EhAtivo),
                FiltroStatus.Ambos => CriarConsulta(),
                _ => throw new ArgumentOutOfRangeException(nameof(filtroStatus), "Filtro informado não gerenciado.")
            };
        }

        public async Task<IEnumerable<TEntidade>> ObterListagem(CancellationToken cancelamento = default)
        {
            return await Obter().OrderBy(objeto => objeto.Id).ToListAsync(cancelamento);
        }

        public Task<TEntidade> ObterPeloId(int id, CancellationToken cancelamento = default)
            => ObterPeloIdAsync(id, cancelamento);

        public async Task<TEntidade> ObterPeloIdAsync(int id, CancellationToken cancelamento = default)
        {
            return await CriarConsulta().SingleOrDefaultAsync(objeto => objeto.Id == id, cancelamento)
                ?? throw new InvalidOperationException(
                    $"Não foi possível encontrar o objeto do tipo '{typeof(TEntidade).Name}' no banco de dados. ID: {id}.");
        }

        public Task Inserir(TEntidade objeto, CancellationToken cancelamento = default)
            => InserirAsync(objeto, cancelamento);

        public virtual async Task InserirAsync(TEntidade objeto, CancellationToken cancelamento = default)
        {
            Contexto.Set<TEntidade>().Add(objeto);
            await Contexto.SaveChangesAsync(cancelamento);
        }

        public Task Alterar(TEntidade objeto, CancellationToken cancelamento = default)
            => AlterarAsync(objeto, cancelamento);

        public virtual async Task AlterarAsync(TEntidade objeto, CancellationToken cancelamento = default)
        {
            PrepararAlteracao(objeto, DateTime.UtcNow);
            await Contexto.SaveChangesAsync(cancelamento);
        }

        public virtual async Task AlterarVariosAsync(IEnumerable<TEntidade> objetos,
            CancellationToken cancelamento = default)
        {
            var dataAtualizacao = DateTime.UtcNow;
            foreach (var objeto in objetos)
                PrepararAlteracao(objeto, dataAtualizacao);
            await Contexto.SaveChangesAsync(cancelamento);
        }

        private void PrepararAlteracao(TEntidade objeto, DateTime dataAtualizacao)
        {
            var registro = Contexto.Entry(objeto);
            registro.State = EntityState.Modified;
            registro.Property(entidade => entidade.DataAtualizacao).CurrentValue = dataAtualizacao;
        }

        public Task Remover(int id, CancellationToken cancelamento = default)
            => RemoverAsync(id, cancelamento);

        public virtual async Task RemoverAsync(int id, CancellationToken cancelamento = default)
        {
            var objeto = await ObterPeloIdAsync(id, cancelamento);
            await RemoverPorObjeto(objeto, cancelamento);
        }

        public virtual async Task RemoverPorObjeto(TEntidade objeto, CancellationToken cancelamento = default)
        {
            if (objeto is null)
                return;
            Contexto.Set<TEntidade>().Remove(objeto);
            await Contexto.SaveChangesAsync(cancelamento);
        }
    }
}
