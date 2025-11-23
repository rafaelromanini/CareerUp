using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using CareerUp.Trainer.Models;

namespace CareerUp.Trainer;

class Program
{
    // Caminho do dataset de treinamento
    private static readonly string TrainingDataPath = Path.Combine("Data", "training-data.csv");
    
    // Caminho onde o modelo treinado será salvo
    private static readonly string ModelPath = "CareerModel.zip";

    static void Main(string[] args)
    {
        Console.WriteLine("=== CareerUp ML.NET Trainer ===");
        Console.WriteLine($"Iniciando treinamento do modelo de recomendação de carreira...\n");

        // 1. Criar contexto do ML.NET
        var mlContext = new MLContext(seed: 42);

        // 2. Carregar dados de treinamento
        Console.WriteLine($"📂 Carregando dados de: {TrainingDataPath}");
        IDataView dataView = mlContext.Data.LoadFromTextFile<CareerData>(
            path: TrainingDataPath,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true
        );

        var preview = dataView.Preview(maxRows: 100);
        Console.WriteLine($"✅ {preview.RowView.Length} registros carregados para preview");
        Console.WriteLine($"   Colunas: Cargo, HabilidadePrimaria, HabilidadeSecundaria, HabilidadeTerciaria, Recomendacao\n");

        // 3. Construir pipeline de treinamento
        Console.WriteLine("🔧 Construindo pipeline de transformação...");
        
        var pipeline = mlContext.Transforms.Text
            // Concatenar todas as features de entrada em um único vetor de texto
            .FeaturizeText(
                outputColumnName: "Features",
                new TextFeaturizingEstimator.Options
                {
                    KeepDiacritics = false,
                    KeepPunctuations = false,
                    CaseMode = TextNormalizingEstimator.CaseMode.Lower
                },
                nameof(CareerData.Cargo),
                nameof(CareerData.HabilidadePrimaria),
                nameof(CareerData.HabilidadeSecundaria),
                nameof(CareerData.HabilidadeTerciaria)
            )
            // Mapear a coluna de recomendação como label
            .Append(mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "Label",
                inputColumnName: nameof(CareerData.Recomendacao)
            ))
            // Usar algoritmo de classificação multiclasse (SDCA)
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "Label",
                featureColumnName: "Features"
            ))
            // Converter label de volta para texto
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                outputColumnName: "PredictedLabel",
                inputColumnName: "PredictedLabel"
            ));

        Console.WriteLine("✅ Pipeline construído com sucesso!");
        Console.WriteLine("   - Featurization de texto (TF-IDF)");
        Console.WriteLine("   - Normalização de texto (lowercase, sem pontuação)");
        Console.WriteLine("   - Algoritmo: SDCA Maximum Entropy (Classificação Multiclasse)\n");

        // 4. Treinar o modelo
        Console.WriteLine("🎓 Treinando modelo ML.NET...");
        var startTime = DateTime.Now;
        
        ITransformer model = pipeline.Fit(dataView);
        
        var trainingTime = DateTime.Now - startTime;
        Console.WriteLine($"✅ Modelo treinado com sucesso em {trainingTime.TotalSeconds:F2} segundos!\n");

        // 5. Avaliar o modelo (opcional - usando cross-validation)
        Console.WriteLine("📊 Avaliando performance do modelo...");
        var predictions = model.Transform(dataView);
        var metrics = mlContext.MulticlassClassification.Evaluate(
            predictions,
            labelColumnName: "Label",
            scoreColumnName: "Score"
        );

        Console.WriteLine($"   📈 Métricas de Performance:");
        Console.WriteLine($"      - Micro Accuracy: {metrics.MicroAccuracy:P2}");
        Console.WriteLine($"      - Macro Accuracy: {metrics.MacroAccuracy:P2}");
        Console.WriteLine($"      - Log Loss: {metrics.LogLoss:F4}");
        Console.WriteLine($"      - Log Loss Reduction: {metrics.LogLossReduction:F4}\n");

        // 6. Salvar modelo treinado
        var modelDirectory = Path.GetDirectoryName(ModelPath);
        if (!string.IsNullOrEmpty(modelDirectory) && !Directory.Exists(modelDirectory))
        {
            Directory.CreateDirectory(modelDirectory);
        }

        Console.WriteLine($"💾 Salvando modelo em: {ModelPath}");
        mlContext.Model.Save(model, dataView.Schema, ModelPath);
        
        var fileInfo = new FileInfo(ModelPath);
        Console.WriteLine($"✅ Modelo salvo com sucesso! Tamanho: {fileInfo.Length / 1024.0:F2} KB\n");

        // 7. Testar o modelo com uma predição
        Console.WriteLine("🧪 Testando predição com exemplo...");
        var predictionEngine = mlContext.Model.CreatePredictionEngine<CareerData, CareerPrediction>(model);

        var testSample = new CareerData
        {
            Cargo = "Desenvolvedor",
            HabilidadePrimaria = "C#",
            HabilidadeSecundaria = ".NET Core",
            HabilidadeTerciaria = "SQL"
        };

        var prediction = predictionEngine.Predict(testSample);
        
        Console.WriteLine($"   📝 Input:");
        Console.WriteLine($"      Cargo: {testSample.Cargo}");
        Console.WriteLine($"      Habilidades: {testSample.HabilidadePrimaria}, {testSample.HabilidadeSecundaria}, {testSample.HabilidadeTerciaria}");
        Console.WriteLine($"\n   🎯 Recomendação Prevista:");
        Console.WriteLine($"      {prediction.Recomendacao[..Math.Min(200, prediction.Recomendacao.Length)]}...\n");

        // 8. Resumo final
        Console.WriteLine("========================================");
        Console.WriteLine("✨ Treinamento Concluído com Sucesso! ✨");
        Console.WriteLine("========================================");
        Console.WriteLine($"📦 Modelo: {Path.GetFileName(ModelPath)}");
        Console.WriteLine($"📂 Localização: {modelDirectory}");
        Console.WriteLine($"🎯 Accuracy: {metrics.MicroAccuracy:P2}");
        Console.WriteLine($"⏱️  Tempo de Treinamento: {trainingTime.TotalSeconds:F2}s");
        Console.WriteLine("\n🚀 O modelo está pronto para ser usado pela API CareerUp!");
        Console.WriteLine("   Copie o arquivo 'CareerModel.zip' para o projeto da API.\n");
    }
}
