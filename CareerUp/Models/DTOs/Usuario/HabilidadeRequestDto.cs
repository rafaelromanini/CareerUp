using System.ComponentModel.DataAnnotations;

namespace CareerUp.Models.DTOs.Usuario
{
    /// <summary>
    /// DTO para requisição de habilidades
    /// </summary>
    public class HabilidadeRequestDto
    {
        /// <summary>
        /// Habilidade primária
        /// </summary>
        [Required(ErrorMessage = "Habilidade primária é obrigatória")]
        [StringLength(200, ErrorMessage = "Habilidade primária deve ter no máximo 200 caracteres")]
        public string HabilidadePrimaria { get; set; } = string.Empty;

        /// <summary>
        /// Habilidade secundária
        /// </summary>
        [Required(ErrorMessage = "Habilidade secundária é obrigatória")]
        [StringLength(200, ErrorMessage = "Habilidade secundária deve ter no máximo 200 caracteres")]
        public string HabilidadeSecundaria { get; set; } = string.Empty;

        /// <summary>
        /// Habilidade terciária
        /// </summary>
        [Required(ErrorMessage = "Habilidade terciária é obrigatória")]
        [StringLength(200, ErrorMessage = "Habilidade terciária deve ter no máximo 200 caracteres")]
        public string HabilidadeTerciaria { get; set; } = string.Empty;
    }
}
