using Microsoft.ML.Data;

namespace CareerUp.Models.ML;

/// <summary>
/// Classe de saída do modelo ML.NET.
/// Contém a recomendação de carreira prevista pelo modelo.
/// </summary>
public class CareerPrediction
{
    /// <summary>
    /// Recomendação de carreira prevista pelo modelo ML.NET.
    /// Texto estruturado com cursos, vagas e plano de desenvolvimento.
    /// </summary>
    [ColumnName("PredictedLabel")]
    public string Recomendacao { get; set; } = string.Empty;

    /// <summary>
    /// Score/confiança da predição (opcional).
    /// </summary>
    public float[]? Score { get; set; }
}
