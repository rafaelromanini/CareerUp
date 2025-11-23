using CareerUp.Models;

namespace CareerUp.Tests.Models;

/// <summary>
/// Teste 2: Valida regra de negócio - Habilidades devem conter 3 campos obrigatórios
/// </summary>
public class HabilidadeTests
{
    [Fact]
    public void Habilidade_DeveTerTresCamposPreenchidos()
    {
        // Arrange & Act
        var habilidade = new Habilidade
        {
            IdUsuario = 1,
            HabilidadePrimaria = "C#",
            HabilidadeSecundaria = "SQL Server",
            HabilidadeTerciaria = "Azure DevOps"
        };

        // Assert
        Assert.Equal("C#", habilidade.HabilidadePrimaria);
        Assert.Equal("SQL Server", habilidade.HabilidadeSecundaria);
        Assert.Equal("Azure DevOps", habilidade.HabilidadeTerciaria);
        Assert.False(string.IsNullOrEmpty(habilidade.HabilidadePrimaria));
        Assert.False(string.IsNullOrEmpty(habilidade.HabilidadeSecundaria));
        Assert.False(string.IsNullOrEmpty(habilidade.HabilidadeTerciaria));
    }
}
