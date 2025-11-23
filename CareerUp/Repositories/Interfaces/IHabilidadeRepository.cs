using CareerUp.Models;

namespace CareerUp.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de Habilidade
    /// </summary>
    public interface IHabilidadeRepository
    {
        Task<Habilidade?> GetByUsuarioIdAsync(long usuarioId);
        Task<Habilidade> CreateAsync(Habilidade habilidade);
        Task<Habilidade> UpdateAsync(Habilidade habilidade);
    }
}
