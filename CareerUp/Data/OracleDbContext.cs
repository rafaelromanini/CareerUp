using Microsoft.EntityFrameworkCore;
using CareerUp.Models;
using CareerUp.Data.Mappings;

namespace CareerUp.Data
{
    /// <summary>
    /// Contexto do banco de dados Oracle para o CareerUp
    /// </summary>
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet de Usuários
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// DbSet de Habilidades
        /// </summary>
        public DbSet<Habilidade> Habilidades { get; set; }

        /// <summary>
        /// DbSet de Logins de Usuários
        /// </summary>
        public DbSet<LoginUsuario> LoginsUsuarios { get; set; }

        /// <summary>
        /// DbSet de Recomendações
        /// </summary>
        public DbSet<Recomendacao> Recomendacoes { get; set; }

        /// <summary>
        /// Configuração dos mapeamentos das entidades
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar configurações de mapeamento
            modelBuilder.ApplyConfiguration(new UsuarioMapping());
            modelBuilder.ApplyConfiguration(new HabilidadeMapping());
            modelBuilder.ApplyConfiguration(new LoginUsuarioMapping());
            modelBuilder.ApplyConfiguration(new RecomendacaoMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
