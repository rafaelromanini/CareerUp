using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerUp.Models
{
    /// <summary>
    /// Armazena as três principais habilidades de cada usuário
    /// </summary>
    [Table("tb_habilidade")]
    public class Habilidade
    {
        /// <summary>
        /// ID do usuário (chave primária compartilhada)
        /// </summary>
        [Key]
        [Column("id_usuario")]
        [ForeignKey("Usuario")]
        public long IdUsuario { get; set; }

        /// <summary>
        /// Principal habilidade
        /// </summary>
        [Required]
        [Column("habilidade_primaria")]
        public string HabilidadePrimaria { get; set; } = string.Empty;

        /// <summary>
        /// Segunda habilidade
        /// </summary>
        [Required]
        [Column("habilidade_secundaria")]
        public string HabilidadeSecundaria { get; set; } = string.Empty;

        /// <summary>
        /// Terceira habilidade
        /// </summary>
        [Required]
        [Column("habilidade_terciaria")]
        public string HabilidadeTerciaria { get; set; } = string.Empty;

        // Navigation Property

        /// <summary>
        /// Usuário associado (relacionamento 1:1)
        /// </summary>
        public Usuario Usuario { get; set; } = null!;
    }
}
