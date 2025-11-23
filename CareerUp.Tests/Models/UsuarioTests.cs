using CareerUp.Models;
using CareerUp.Models.Enums;

namespace CareerUp.Tests.Models;

/// <summary>
/// Teste 1: Valida regra de negócio - Propriedades obrigatórias do Usuario
/// </summary>
public class UsuarioTests
{
    [Fact]
    public void Usuario_DeveTerPropriedadesObrigatorias()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            IdUsuario = 1,
            NomeUsuario = "João Silva",
            Cpf = "12345678900",
            Email = "joao@email.com",
            Cargo = "Desenvolvedor Backend",
            Papel = PapelUsuario.USUARIO
        };

        // Assert
        Assert.NotNull(usuario);
        Assert.Equal(1, usuario.IdUsuario);
        Assert.Equal("João Silva", usuario.NomeUsuario);
        Assert.Equal("12345678900", usuario.Cpf);
        Assert.Equal("joao@email.com", usuario.Email);
        Assert.Equal("Desenvolvedor Backend", usuario.Cargo);
        Assert.Equal(PapelUsuario.USUARIO, usuario.Papel);
    }

    [Fact]
    public void Usuario_DevePermitirPapelGerente()
    {
        // Arrange & Act
        var gerente = new Usuario
        {
            IdUsuario = 2,
            NomeUsuario = "Maria Admin",
            Cpf = "98765432100",
            Email = "maria@email.com",
            Cargo = "Gerente de Projetos",
            Papel = PapelUsuario.GERENTE
        };

        // Assert
        Assert.Equal(PapelUsuario.GERENTE, gerente.Papel);
    }
}
