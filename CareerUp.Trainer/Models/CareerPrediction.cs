using Microsoft.ML.Data;

namespace CareerUp.Trainer.Models;

/// <summary>
/// Classe que representa a saída (predição) do modelo ML.NET.
/// Contém a recomendação de carreira gerada pelo modelo treinado.
/// </summary>
public class CareerPrediction
{
    /// <summary>
    /// Texto da recomendação prevista pelo modelo.
    /// Inclui sugestões de cursos, vagas e plano de desenvolvimento.
    /// </summary>
    [ColumnName("PredictedLabel")]
    public string Recomendacao { get; set; } = string.Empty;

    /// <summary>
    /// Score/confiança da predição (opcional, para futuras melhorias).
    /// </summary>
    public float[]? Score { get; set; }
}
