using Microsoft.ML.Data;

namespace CareerUp.Trainer.Models;

/// <summary>
/// Classe que representa os dados de entrada para treinamento do modelo.
/// Combina cargo atual e habilidades do usuário para gerar recomendações.
/// </summary>
public class CareerData
{
    /// <summary>
    /// Cargo atual do usuário (ex: "Desenvolvedor", "Analista de Dados").
    /// </summary>
    [LoadColumn(0)]
    public string Cargo { get; set; } = string.Empty;

    /// <summary>
    /// Principal habilidade do usuário (ex: "C#", "Python").
    /// </summary>
    [LoadColumn(1)]
    public string HabilidadePrimaria { get; set; } = string.Empty;

    /// <summary>
    /// Segunda habilidade do usuário (ex: ".NET Core", "Machine Learning").
    /// </summary>
    [LoadColumn(2)]
    public string HabilidadeSecundaria { get; set; } = string.Empty;

    /// <summary>
    /// Terceira habilidade do usuário (ex: "SQL", "TensorFlow").
    /// </summary>
    [LoadColumn(3)]
    public string HabilidadeTerciaria { get; set; } = string.Empty;

    /// <summary>
    /// Recomendação gerada pela IA (texto completo estruturado).
    /// Este é o campo que o modelo vai aprender a prever.
    /// </summary>
    [LoadColumn(4)]
    public string Recomendacao { get; set; } = string.Empty;
}
