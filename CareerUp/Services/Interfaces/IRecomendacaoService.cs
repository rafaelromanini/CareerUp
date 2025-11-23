using CareerUp.Models;
using CareerUp.Models.DTOs.Recomendacao;

namespace CareerUp.Services.Interfaces;

/// <summary>
/// Interface do serviço de recomendações.
/// </summary>
public interface IRecomendacaoService
{
    /// <summary>
    /// Gera uma nova recomendação para o usuário autenticado.
    /// </summary>
    Task<RecomendacaoResponseDto> GerarRecomendacaoAsync(long idUsuario);

    /// <summary>
    /// Busca uma recomendação por ID.
    /// </summary>
    Task<RecomendacaoResponseDto?> GetByIdAsync(long idRecomendacao, long idUsuarioAutenticado);

    /// <summary>
    /// Lista recomendações do usuário com paginação.
    /// </summary>
    Task<(List<RecomendacaoResponseDto> items, int totalCount)> GetByUsuarioIdAsync(
        long idUsuario, 
        long idUsuarioAutenticado, 
        int pageNumber, 
        int pageSize);

    /// <summary>
    /// Lista recomendações do usuário filtradas por mês com paginação.
    /// </summary>
    Task<(List<RecomendacaoResponseDto> items, int totalCount)> GetByUsuarioIdAndMonthAsync(
        long idUsuario, 
        long idUsuarioAutenticado, 
        int mes,
        int pageNumber, 
        int pageSize);

    /// <summary>
    /// Exclui uma recomendação.
    /// </summary>
    Task<bool> DeleteAsync(long idRecomendacao, long idUsuarioAutenticado);
}
