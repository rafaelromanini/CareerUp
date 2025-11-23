using CareerUp.Models;

namespace CareerUp.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de Usuário
    /// </summary>
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(long id);
        Task<Usuario?> GetByIdWithDetailsAsync(long id);
        Task<Usuario?> GetByCpfAsync(string cpf);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllWithDetailsAsync();
        Task<int> GetCountAsync();
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}
