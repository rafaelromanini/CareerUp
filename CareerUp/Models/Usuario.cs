using CareerUp.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerUp.Models
{
    /// <summary>
    /// Entidade central que representa os usuários da plataforma
    /// </summary>
    [Table("tb_usuario")]
    public class Usuario
    {
        /// <summary>
        /// ID sequencial gerado automaticamente
        /// </summary>
        [Key]
        [Column("id_usuario")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdUsuario { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required]
        [Column("nome_usuario")]
        [StringLength(255)]
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário (único)
        /// </summary>
        [Required]
        [Column("cpf")]
        [StringLength(30)]
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário (único)
        /// </summary>
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Cargo atual do usuário
        /// </summary>
        [Required]
        [Column("cargo")]
        public string Cargo { get; set; } = string.Empty;

        /// <summary>
        /// Papel/função no sistema
        /// </summary>
        [Required]
        [Column("papel")]
        public PapelUsuario Papel { get; set; }

        // Navigation Properties

        /// <summary>
        /// Habilidades do usuário (relacionamento 1:1)
        /// </summary>
        public Habilidade? Habilidade { get; set; }

        /// <summary>
        /// Credenciais de login (relacionamento 1:1)
        /// </summary>
        public LoginUsuario? LoginUsuario { get; set; }

        /// <summary>
        /// Recomendações de carreira (relacionamento 1:N)
        /// </summary>
        public ICollection<Recomendacao> Recomendacoes { get; set; } = new List<Recomendacao>();
    }
}
