using CareerUp.Models.DTOs.Usuario;

namespace CareerUp.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para resposta de login
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Token de acesso JWT
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Dados do usuário autenticado
        /// </summary>
        public UsuarioResponseDto Usuario { get; set; } = null!;
    }
}
