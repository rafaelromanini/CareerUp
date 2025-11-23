namespace CareerUp.Models.DTOs.Recomendacao;

/// <summary>
/// DTO para requisição de geração de recomendação.
/// Não requer dados adicionais pois usará os dados do usuário autenticado.
/// </summary>
public class GerarRecomendacaoRequestDto
{
    // Vazio intencionalmente - usa dados do usuário autenticado (cargo + habilidades)
}
