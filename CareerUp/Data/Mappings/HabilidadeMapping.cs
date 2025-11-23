using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerUp.Models;

namespace CareerUp.Data.Mappings
{
    /// <summary>
    /// Configuração do mapeamento da entidade Habilidade para a tabela Oracle
    /// </summary>
    public class HabilidadeMapping : IEntityTypeConfiguration<Habilidade>
    {
        public void Configure(EntityTypeBuilder<Habilidade> builder)
        {
            // Configuração da tabela
            builder.ToTable("tb_habilidade");

            // Chave primária (compartilhada com Usuario)
            builder.HasKey(h => h.IdUsuario);

            builder.Property(h => h.IdUsuario)
                .HasColumnName("id_usuario");

            // Propriedades
            builder.Property(h => h.HabilidadePrimaria)
                .HasColumnName("habilidade_primaria")
                .IsRequired();

            builder.Property(h => h.HabilidadeSecundaria)
                .HasColumnName("habilidade_secundaria")
                .IsRequired();

            builder.Property(h => h.HabilidadeTerciaria)
                .HasColumnName("habilidade_terciaria")
                .IsRequired();

            // Relacionamento 1:1 com Usuario
            builder.HasOne(h => h.Usuario)
                .WithOne(u => u.Habilidade)
                .HasForeignKey<Habilidade>(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
