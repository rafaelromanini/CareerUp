using CareerUp.Models;
using CareerUp.Models.Enums;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services;
using CareerUp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CareerUp.Tests.Services;

/// <summary>
/// Testes para RecomendacaoService - Validam regras de negócio críticas
/// </summary>
public class RecomendacaoServiceTests
{
    private readonly Mock<IRecomendacaoRepository> _mockRecomendacaoRepo;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
    private readonly Mock<IMLPredictionService> _mockMLService;
    private readonly Mock<ILogger<RecomendacaoService>> _mockLogger;
    private readonly RecomendacaoService _service;

    public RecomendacaoServiceTests()
    {
        _mockRecomendacaoRepo = new Mock<IRecomendacaoRepository>();
        _mockUsuarioRepo = new Mock<IUsuarioRepository>();
        _mockMLService = new Mock<IMLPredictionService>();
        _mockLogger = new Mock<ILogger<RecomendacaoService>>();
        
        _service = new RecomendacaoService(
            _mockRecomendacaoRepo.Object,
            _mockUsuarioRepo.Object,
            _mockMLService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GerarRecomendacaoAsync_DeveGerarRecomendacao_QuandoUsuarioExiste()
    {
        // Arrange
        var habilidade = new Habilidade
        {
            IdUsuario = 1,
            HabilidadePrimaria = "C#",
            HabilidadeSecundaria = "SQL Server",
            HabilidadeTerciaria = "Azure"
        };

        var usuario = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "João Silva",
            Cargo = "Desenvolvedor Backend",
            Habilidade = habilidade
        };

        var recomendacaoGerada = new Recomendacao
        {
            IdRecomendacao = 1,
            DataGeracao = DateTime.Now,
            ResultadoIa = "Recomendação de cursos: .NET Avançado, Microserviços",
            IdUsuario = 1,
            Usuario = usuario
        };

        _mockUsuarioRepo.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(usuario);
        
        _mockMLService.Setup(m => m.PredictCareerRecommendation(
            "Desenvolvedor Backend", "C#", "SQL Server", "Azure"))
            .Returns("Recomendação de cursos: .NET Avançado, Microserviços");
        
        _mockRecomendacaoRepo.Setup(r => r.AddAsync(It.IsAny<Recomendacao>()))
            .ReturnsAsync(recomendacaoGerada);

        // Act
        var resultado = await _service.GerarRecomendacaoAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.IdRecomendacao);
        Assert.Contains("Microserviços", resultado.ResultadoIa);
        _mockMLService.Verify(m => m.PredictCareerRecommendation(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockRecomendacaoRepo.Verify(r => r.AddAsync(It.IsAny<Recomendacao>()), Times.Once);
    }

    [Fact]
    public async Task GerarRecomendacaoAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        // Arrange
        _mockUsuarioRepo.Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GerarRecomendacaoAsync(999)
        );
        
        _mockMLService.Verify(m => m.PredictCareerRecommendation(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornar_ApenasRecomendacaoDoProprioUsuario()
    {
        // Arrange - Usuario comum tentando acessar sua própria recomendação
        var usuario = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "João Silva",
            Papel = PapelUsuario.USUARIO
        };

        var recomendacao = new Recomendacao
        {
            IdRecomendacao = 1,
            IdUsuario = 1,
            ResultadoIa = "Recomendação do João",
            Usuario = usuario
        };

        _mockRecomendacaoRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(recomendacao);
        
        _mockUsuarioRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(usuario);

        // Act
        var resultado = await _service.GetByIdAsync(1, 1); // usuário 1 acessando recomendação 1

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.IdRecomendacao);
        Assert.Equal("João Silva", resultado.NomeUsuario);
    }

    [Fact]
    public async Task GetByIdAsync_DeveLancarUnauthorized_QuandoUsuarioTentaAcessarRecomendacaoDeOutro()
    {
        // Arrange - Usuario comum tentando acessar recomendação de outro usuário
        var usuarioOutro = new Usuario
        {
            IdUsuario = 2,
            NomeUsuario = "Maria Santos",
            Papel = PapelUsuario.USUARIO
        };

        var usuarioAutenticado = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "João Silva",
            Papel = PapelUsuario.USUARIO
        };

        var recomendacao = new Recomendacao
        {
            IdRecomendacao = 5,
            IdUsuario = 2, // Recomendação de outro usuário
            ResultadoIa = "Recomendação da Maria",
            Usuario = usuarioOutro
        };

        _mockRecomendacaoRepo.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(recomendacao);
        
        _mockUsuarioRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(usuarioAutenticado);

        // Act & Assert - Usuario 1 tentando acessar recomendação do usuario 2
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _service.GetByIdAsync(5, 1)
        );
    }

    [Fact]
    public async Task GetByIdAsync_DevePermitir_GerenteAcessarQualquerRecomendacao()
    {
        // Arrange - Gerente pode acessar recomendação de qualquer usuário
        var usuarioComum = new Usuario
        {
            IdUsuario = 2,
            NomeUsuario = "Maria Santos",
            Papel = PapelUsuario.USUARIO
        };

        var gerente = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "Admin Boss",
            Papel = PapelUsuario.GERENTE
        };

        var recomendacao = new Recomendacao
        {
            IdRecomendacao = 5,
            IdUsuario = 2,
            ResultadoIa = "Recomendação da Maria",
            Usuario = usuarioComum
        };

        _mockRecomendacaoRepo.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(recomendacao);
        
        _mockUsuarioRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(gerente);

        // Act - Gerente acessando recomendação de outro usuário
        var resultado = await _service.GetByIdAsync(5, 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(5, resultado.IdRecomendacao);
        Assert.Equal("Maria Santos", resultado.NomeUsuario);
    }
}
