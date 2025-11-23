using CareerUp.Models.DTOs.Common;

namespace CareerUp.Models.DTOs.Recomendacao;

/// <summary>
/// DTO de resposta contendo uma recomendação de carreira.
/// </summary>
public class RecomendacaoResponseDto
{
    /// <summary>
    /// ID da recomendação.
    /// </summary>
    public long IdRecomendacao { get; set; }

    /// <summary>
    /// Data e hora em que a recomendação foi gerada.
    /// </summary>
    public DateTime DataGeracao { get; set; }

    /// <summary>
    /// Texto completo da recomendação gerada pela IA.
    /// Inclui cursos, vagas e plano de desenvolvimento.
    /// </summary>
    public string ResultadoIa { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário que recebeu a recomendação.
    /// </summary>
    public long IdUsuario { get; set; }

    /// <summary>
    /// Nome do usuário que recebeu a recomendação.
    /// </summary>
    public string NomeUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Cargo do usuário na época da geração.
    /// </summary>
    public string Cargo { get; set; } = string.Empty;

    /// <summary>
    /// Links HATEOAS para navegação.
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
