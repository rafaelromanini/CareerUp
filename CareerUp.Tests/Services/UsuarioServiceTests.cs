using CareerUp.Models;
using CareerUp.Models.DTOs.Usuario;
using CareerUp.Models.Enums;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CareerUp.Tests.Services;

/// <summary>
/// Testes para UsuarioService - Validam regras de negócio de usuários
/// </summary>
public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
    private readonly Mock<IHabilidadeRepository> _mockHabilidadeRepo;
    private readonly Mock<ILogger<UsuarioService>> _mockLogger;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _mockUsuarioRepo = new Mock<IUsuarioRepository>();
        _mockHabilidadeRepo = new Mock<IHabilidadeRepository>();
        _mockLogger = new Mock<ILogger<UsuarioService>>();
        
        _service = new UsuarioService(
            _mockUsuarioRepo.Object,
            _mockHabilidadeRepo.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task UpdateCargoAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        // Arrange
        _mockUsuarioRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert - Regra de negócio: não pode atualizar cargo de usuário inexistente
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.UpdateCargoAsync(999, "Gerente")
        );
        
        _mockUsuarioRepo.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task UpdateHabilidadesAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        // Arrange
        _mockUsuarioRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Usuario?)null);

        var dto = new HabilidadeRequestDto
        {
            HabilidadePrimaria = "Java",
            HabilidadeSecundaria = "Spring",
            HabilidadeTerciaria = "Docker"
        };

        // Act & Assert - Regra de negócio: não pode atualizar habilidades de usuário inexistente
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.UpdateHabilidadesAsync(999, dto)
        );
        
        _mockHabilidadeRepo.Verify(r => r.UpdateAsync(It.IsAny<Habilidade>()), Times.Never);
    }
}
