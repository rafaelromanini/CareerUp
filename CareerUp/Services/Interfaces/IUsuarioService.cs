using CareerUp.Models.DTOs.Usuario;

namespace CareerUp.Services.Interfaces
{
    /// <summary>
    /// Interface para serviço de usuários
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtém usuário por ID
        /// </summary>
        Task<UsuarioResponseDto> GetByIdAsync(long id);

        /// <summary>
        /// Obtém todos os usuários com paginação
        /// </summary>
        Task<(IEnumerable<UsuarioResponseDto> Usuarios, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Atualiza cargo de um usuário
        /// </summary>
        Task<UsuarioResponseDto> UpdateCargoAsync(long id, string cargo);

        /// <summary>
        /// Atualiza habilidades de um usuário
        /// </summary>
        Task<UsuarioResponseDto> UpdateHabilidadesAsync(long id, HabilidadeRequestDto habilidadesDto);

        /// <summary>
        /// Exclui um usuário
        /// </summary>
        Task DeleteAsync(long id);
    }
}
