using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerUp.Models;

namespace CareerUp.Data.Mappings
{
    /// <summary>
    /// Configuração do mapeamento da entidade Recomendacao para a tabela Oracle
    /// </summary>
    public class RecomendacaoMapping : IEntityTypeConfiguration<Recomendacao>
    {
        public void Configure(EntityTypeBuilder<Recomendacao> builder)
        {
            // Configuração da tabela
            builder.ToTable("tb_recomendacao");

            // Chave primária
            builder.HasKey(r => r.IdRecomendacao);

            builder.Property(r => r.IdRecomendacao)
                .HasColumnName("id_recomendacao")
                .ValueGeneratedOnAdd();

            // Propriedades
            builder.Property(r => r.DataGeracao)
                .HasColumnName("data_geracao")
                .IsRequired();

            builder.Property(r => r.ResultadoIa)
                .HasColumnName("resultado_ia")
                .HasColumnType("CLOB")
                .IsRequired();

            builder.Property(r => r.IdUsuario)
                .HasColumnName("id_usuario")
                .IsRequired();

            // Índice para consultas por usuário
            builder.HasIndex(r => r.IdUsuario)
                .HasDatabaseName("idx_recomendacao_usuario");

            // Índice para consultas por data
            builder.HasIndex(r => r.DataGeracao)
                .HasDatabaseName("idx_recomendacao_data");

            // Relacionamento N:1 com Usuario
            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.Recomendacoes)
                .HasForeignKey(r => r.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
