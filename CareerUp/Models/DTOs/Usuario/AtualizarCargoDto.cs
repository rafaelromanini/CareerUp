using System.ComponentModel.DataAnnotations;

namespace CareerUp.Models.DTOs.Usuario
{
    /// <summary>
    /// DTO para atualização de cargo
    /// </summary>
    public class AtualizarCargoDto
    {
        /// <summary>
        /// Novo cargo do usuário
        /// </summary>
        [Required(ErrorMessage = "Cargo é obrigatório")]
        [StringLength(200, ErrorMessage = "Cargo deve ter no máximo 200 caracteres")]
        public string Cargo { get; set; } = string.Empty;
    }
}
