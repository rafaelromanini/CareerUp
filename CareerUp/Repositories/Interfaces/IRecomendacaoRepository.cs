using CareerUp.Models;

namespace CareerUp.Repositories.Interfaces;

/// <summary>
/// Interface do repositório de recomendações.
/// </summary>
public interface IRecomendacaoRepository
{
    /// <summary>
    /// Adiciona uma nova recomendação ao banco de dados.
    /// </summary>
    Task<Recomendacao> AddAsync(Recomendacao recomendacao);

    /// <summary>
    /// Busca uma recomendação por ID.
    /// </summary>
    Task<Recomendacao?> GetByIdAsync(long idRecomendacao);

    /// <summary>
    /// Lista todas as recomendações de um usuário com paginação.
    /// </summary>
    Task<(List<Recomendacao> items, int totalCount)> GetByUsuarioIdAsync(long idUsuario, int pageNumber, int pageSize);

    /// <summary>
    /// Exclui uma recomendação.
    /// </summary>
    Task<bool> DeleteAsync(long idRecomendacao);

    /// <summary>
    /// Verifica se uma recomendação existe.
    /// </summary>
    Task<bool> ExistsAsync(long idRecomendacao);
}
