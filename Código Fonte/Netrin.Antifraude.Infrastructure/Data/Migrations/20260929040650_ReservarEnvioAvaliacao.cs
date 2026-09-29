using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Netrin.Antifraude.Infrastructure.Data.Migrations
{
    public partial class ReservarEnvioAvaliacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnvioParaAvaliacaoSolicitado",
                table: "Transacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE "Transacoes" AS transacao
                SET "EnvioParaAvaliacaoSolicitado" = TRUE
                WHERE EXISTS (
                    SELECT 1 FROM "Avaliacoes" AS avaliacao
                    WHERE avaliacao."TransacaoId" = transacao."Id"
                ) OR EXISTS (
                    SELECT 1 FROM "StatusTransacoes" AS status
                    WHERE status."TransacaoId" = transacao."Id"
                      AND status."Status" <> 'Pendente'
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvioParaAvaliacaoSolicitado",
                table: "Transacoes");
        }
    }
}
