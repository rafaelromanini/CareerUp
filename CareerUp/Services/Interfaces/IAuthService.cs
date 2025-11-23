using CareerUp.Models.DTOs.Auth;
using CareerUp.Models.DTOs.Usuario;

namespace CareerUp.Services.Interfaces
{
    /// <summary>
    /// Interface para serviço de autenticação
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Realiza login e retorna token JWT
        /// </summary>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        Task<UsuarioResponseDto> RegisterAsync(UsuarioRequestDto usuarioDto);

        /// <summary>
        /// Valida credenciais de usuário
        /// </summary>
        Task<bool> ValidateCredentialsAsync(string login, string senha);
    }
}
