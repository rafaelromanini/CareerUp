using Microsoft.ML.Data;

namespace CareerUp.Models.ML;

/// <summary>
/// Classe de entrada para o modelo ML.NET.
/// Representa os dados do usuário que serão usados para gerar recomendações de carreira.
/// </summary>
public class CareerInput
{
    /// <summary>
    /// Cargo atual do usuário.
    /// </summary>
    [LoadColumn(0)]
    public string Cargo { get; set; } = string.Empty;

    /// <summary>
    /// Principal habilidade do usuário.
    /// </summary>
    [LoadColumn(1)]
    public string HabilidadePrimaria { get; set; } = string.Empty;

    /// <summary>
    /// Segunda habilidade do usuário.
    /// </summary>
    [LoadColumn(2)]
    public string HabilidadeSecundaria { get; set; } = string.Empty;

    /// <summary>
    /// Terceira habilidade do usuário.
    /// </summary>
    [LoadColumn(3)]
    public string HabilidadeTerciaria { get; set; } = string.Empty;

    /// <summary>
    /// Campo vazio usado apenas para compatibilidade com o modelo treinado.
    /// Não é usado durante a inferência.
    /// </summary>
    [LoadColumn(4)]
    public string Recomendacao { get; set; } = string.Empty;
}
