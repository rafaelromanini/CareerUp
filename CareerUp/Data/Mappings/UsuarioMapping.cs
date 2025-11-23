using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerUp.Models;

namespace CareerUp.Data.Mappings
{
    /// <summary>
    /// Configuração do mapeamento da entidade Usuario para a tabela Oracle
    /// </summary>
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            // Configuração da tabela
            builder.ToTable("tb_usuario");

            // Chave primária
            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.IdUsuario)
                .HasColumnName("id_usuario")
                .ValueGeneratedOnAdd();

            // Propriedades
            builder.Property(u => u.NomeUsuario)
                .HasColumnName("nome_usuario")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Cargo)
                .HasColumnName("cargo")
                .IsRequired();

            builder.Property(u => u.Papel)
                .HasColumnName("papel")
                .HasConversion<string>()
                .IsRequired();

            // Índices únicos
            builder.HasIndex(u => u.Cpf)
                .IsUnique()
                .HasDatabaseName("idx_usuario_cpf");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("idx_usuario_email");

            // Relacionamentos
            
            // 1:1 com Habilidade (cascade delete)
            builder.HasOne(u => u.Habilidade)
                .WithOne(h => h.Usuario)
                .HasForeignKey<Habilidade>(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:1 com LoginUsuario (cascade delete)
            builder.HasOne(u => u.LoginUsuario)
                .WithOne(l => l.Usuario)
                .HasForeignKey<LoginUsuario>(l => l.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N com Recomendacao (cascade delete)
            builder.HasMany(u => u.Recomendacoes)
                .WithOne(r => r.Usuario)
                .HasForeignKey(r => r.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
