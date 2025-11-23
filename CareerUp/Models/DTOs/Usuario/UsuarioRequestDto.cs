using System.ComponentModel.DataAnnotations;
using CareerUp.Models.Enums;
using CareerUp.Models.DTOs.Auth;

namespace CareerUp.Models.DTOs.Usuario
{
    /// <summary>
    /// DTO para requisição de criação de usuário
    /// </summary>
    public class UsuarioRequestDto
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "Nome do usuário é obrigatório")]
        [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário
        /// </summary>
        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(30, ErrorMessage = "CPF deve ter no máximo 30 caracteres")]
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Cargo atual do usuário
        /// </summary>
        [Required(ErrorMessage = "Cargo é obrigatório")]
        [StringLength(200, ErrorMessage = "Cargo deve ter no máximo 200 caracteres")]
        public string Cargo { get; set; } = string.Empty;

        /// <summary>
        /// Papel do usuário no sistema.
        /// Valores: 0 = USUARIO, 1 = GERENTE
        /// </summary>
        [Required(ErrorMessage = "Papel é obrigatório")]
        public PapelUsuario Papel { get; set; }

        /// <summary>
        /// Credenciais de login
        /// </summary>
        [Required(ErrorMessage = "Dados de login são obrigatórios")]
        public LoginRequestDto LoginUsuario { get; set; } = null!;

        /// <summary>
        /// Habilidades do usuário
        /// </summary>
        [Required(ErrorMessage = "Habilidades são obrigatórias")]
        public HabilidadeRequestDto Habilidades { get; set; } = null!;
    }
}
