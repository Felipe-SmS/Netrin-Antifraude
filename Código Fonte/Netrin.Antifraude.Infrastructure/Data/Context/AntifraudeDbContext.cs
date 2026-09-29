using Microsoft.EntityFrameworkCore;

namespace Netrin.Antifraude.Infrastructure.Data.Context;

public class AntifraudeDbContext : DbContext
{
    public AntifraudeDbContext(
        DbContextOptions<AntifraudeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<StatusTransacao> StatusTransacoes => Set<StatusTransacao>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.ToTable("Transacoes");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.IdempotencyKey)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(t => t.IdempotencyKey)
                .IsUnique();

            entity.Property(t => t.EnvioParaAvaliacaoSolicitado)
                .IsConcurrencyToken();

            entity.HasOne(t => t.Status)
                .WithOne()
                .HasForeignKey<StatusTransacao>("TransacaoId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Avaliacao)
                .WithOne()
                .HasForeignKey<Avaliacao>("TransacaoId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StatusTransacao>(entity =>
        {
            entity.ToTable("StatusTransacoes");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();
        });

        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.ToTable("Avaliacoes");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Decisao)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
        });
    }
}
