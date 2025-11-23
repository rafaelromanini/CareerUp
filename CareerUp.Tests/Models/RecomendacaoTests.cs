using CareerUp.Models;

namespace CareerUp.Tests.Models;

/// <summary>
/// Teste 3: Valida regra de negócio - Recomendação vinculada a Usuario
/// </summary>
public class RecomendacaoTests
{
    [Fact]
    public void Recomendacao_DeveArmazenarResultadoIaEVinculoComUsuario()
    {
        // Arrange
        var usuario = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "Carlos Developer",
            Cargo = "Desenvolvedor Full Stack"
        };

        var recomendacao = new Recomendacao
        {
            IdRecomendacao = 1,
            DataGeracao = DateTime.Now,
            ResultadoIa = "Recomendação: Curso de .NET Avançado com foco em Microserviços",
            IdUsuario = usuario.IdUsuario,
            Usuario = usuario
        };

        // Assert - Valida propriedades da recomendação
        Assert.NotNull(recomendacao);
        Assert.Equal(1, recomendacao.IdRecomendacao);
        Assert.NotNull(recomendacao.ResultadoIa);
        Assert.Contains("Curso", recomendacao.ResultadoIa);
        
        // Assert - Valida vínculo com usuário
        Assert.Equal(usuario.IdUsuario, recomendacao.IdUsuario);
        Assert.Equal(usuario.NomeUsuario, recomendacao.Usuario.NomeUsuario);
    }
}
