using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CareerUp.Models;

namespace CareerUp.Data.Mappings
{
    /// <summary>
    /// Configuração do mapeamento da entidade LoginUsuario para a tabela Oracle
    /// </summary>
    public class LoginUsuarioMapping : IEntityTypeConfiguration<LoginUsuario>
    {
        public void Configure(EntityTypeBuilder<LoginUsuario> builder)
        {
            // Configuração da tabela
            builder.ToTable("tb_login_usuario");

            // Chave primária (compartilhada com Usuario)
            builder.HasKey(l => l.IdUsuario);

            builder.Property(l => l.IdUsuario)
                .HasColumnName("id_usuario");

            // Propriedades
            builder.Property(l => l.Login)
                .HasColumnName("login")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(l => l.Senha)
                .HasColumnName("senha")
                .HasMaxLength(180)
                .IsRequired();

            // Índice único para login
            builder.HasIndex(l => l.Login)
                .IsUnique()
                .HasDatabaseName("idx_login_usuario");

            // Relacionamento 1:1 com Usuario
            builder.HasOne(l => l.Usuario)
                .WithOne(u => u.LoginUsuario)
                .HasForeignKey<LoginUsuario>(l => l.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
