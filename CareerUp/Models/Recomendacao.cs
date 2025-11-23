using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerUp.Models
{
    /// <summary>
    /// Armazena recomendações de carreira geradas pela IA
    /// </summary>
    [Table("tb_recomendacao")]
    public class Recomendacao
    {
        /// <summary>
        /// ID sequencial da recomendação
        /// </summary>
        [Key]
        [Column("id_recomendacao")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdRecomendacao { get; set; }

        /// <summary>
        /// Data e hora da geração da recomendação
        /// </summary>
        [Required]
        [Column("data_geracao")]
        public DateTime DataGeracao { get; set; }

        /// <summary>
        /// Texto completo da recomendação gerada pela IA
        /// </summary>
        [Required]
        [Column("resultado_ia")]
        public string ResultadoIa { get; set; } = string.Empty;

        /// <summary>
        /// ID do usuário que recebeu a recomendação
        /// </summary>
        [Required]
        [Column("id_usuario")]
        [ForeignKey("Usuario")]
        public long IdUsuario { get; set; }

        // Navigation Property

        /// <summary>
        /// Usuário associado (relacionamento N:1)
        /// </summary>
        public Usuario Usuario { get; set; } = null!;
    }
}
