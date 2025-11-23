using CareerUp.Models.ML;

namespace CareerUp.Services.Interfaces;

/// <summary>
/// Interface do serviço de predição ML.NET.
/// </summary>
public interface IMLPredictionService
{
    /// <summary>
    /// Gera uma recomendação de carreira usando o modelo ML.NET.
    /// </summary>
    /// <param name="cargo">Cargo atual do usuário</param>
    /// <param name="habilidadePrimaria">Principal habilidade</param>
    /// <param name="habilidadeSecundaria">Segunda habilidade</param>
    /// <param name="habilidadeTerciaria">Terceira habilidade</param>
    /// <returns>Texto da recomendação gerada pelo modelo</returns>
    string PredictCareerRecommendation(
        string cargo,
        string habilidadePrimaria,
        string habilidadeSecundaria,
        string habilidadeTerciaria);
}
