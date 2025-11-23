using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerUp.Models
{
    /// <summary>
    /// Credenciais de autenticação do usuário
    /// </summary>
    [Table("tb_login_usuario")]
    public class LoginUsuario
    {
        /// <summary>
        /// ID do usuário (chave primária compartilhada)
        /// </summary>
        [Key]
        [Column("id_usuario")]
        [ForeignKey("Usuario")]
        public long IdUsuario { get; set; }

        /// <summary>
        /// Nome de usuário para login (único)
        /// </summary>
        [Required]
        [Column("login")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Senha criptografada (BCrypt)
        /// </summary>
        [Required]
        [Column("senha")]
        [StringLength(180)]
        public string Senha { get; set; } = string.Empty;

        // Navigation Property

        /// <summary>
        /// Usuário associado (relacionamento 1:1)
        /// </summary>
        public Usuario Usuario { get; set; } = null!;
    }
}
