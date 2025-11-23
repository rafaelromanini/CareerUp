using CareerUp.Models.ML;
using CareerUp.Services.Interfaces;
using Microsoft.ML;

namespace CareerUp.Services;

/// <summary>
/// Serviço de predição ML.NET para recomendações de carreira.
/// Carrega o modelo treinado e faz inferências.
/// </summary>
public class MLPredictionService : IMLPredictionService
{
    private readonly PredictionEngine<CareerInput, CareerPrediction> _predictionEngine;
    private readonly ILogger<MLPredictionService> _logger;

    public MLPredictionService(
        PredictionEngine<CareerInput, CareerPrediction> predictionEngine,
        ILogger<MLPredictionService> logger)
    {
        _predictionEngine = predictionEngine;
        _logger = logger;
    }

    public string PredictCareerRecommendation(
        string cargo,
        string habilidadePrimaria,
        string habilidadeSecundaria,
        string habilidadeTerciaria)
    {
        try
        {
            _logger.LogInformation(
                "Gerando predição para Cargo={Cargo}, Habilidades=[{H1}, {H2}, {H3}]",
                cargo, habilidadePrimaria, habilidadeSecundaria, habilidadeTerciaria);

            var input = new CareerInput
            {
                Cargo = cargo,
                HabilidadePrimaria = habilidadePrimaria,
                HabilidadeSecundaria = habilidadeSecundaria,
                HabilidadeTerciaria = habilidadeTerciaria
            };

            var prediction = _predictionEngine.Predict(input);

            _logger.LogInformation(
                "Predição gerada com sucesso. Tamanho da recomendação: {Length} caracteres",
                prediction.Recomendacao.Length);

            return prediction.Recomendacao;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar predição ML.NET");
            throw new InvalidOperationException("Erro ao gerar recomendação com ML.NET", ex);
        }
    }
}
