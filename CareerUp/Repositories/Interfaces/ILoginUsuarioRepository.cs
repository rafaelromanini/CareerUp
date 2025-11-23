using CareerUp.Models;

namespace CareerUp.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de LoginUsuario
    /// </summary>
    public interface ILoginUsuarioRepository
    {
        Task<LoginUsuario?> GetByLoginAsync(string login);
        Task<LoginUsuario?> GetByUsuarioIdAsync(long usuarioId);
        Task<bool> LoginExistsAsync(string login);
    }
}
