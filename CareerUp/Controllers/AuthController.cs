using CareerUp.Models.DTOs.Auth;
using CareerUp.Models.DTOs.Usuario;
using CareerUp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareerUp.Controllers
{
    /// <summary>
    /// Controller para autenticação de usuários
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza login e retorna token JWT
        /// </summary>
        /// <param name="loginDto">Credenciais de login</param>
        /// <returns>Token de acesso e dados do usuário</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                _logger.LogInformation("Requisição de login recebida para: {Login}", loginDto.Login);
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login falhou: {Message}", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login");
                return StatusCode(500, new { message = "Erro interno ao processar login" });
            }
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="usuarioDto">Dados do novo usuário</param>
        /// <returns>Dados do usuário cadastrado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos ou já existentes</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsuarioResponseDto>> Register([FromBody] UsuarioRequestDto usuarioDto)
        {
            try
            {
                _logger.LogInformation("Requisição de registro recebida para: {NomeUsuario}", usuarioDto.NomeUsuario);
                var response = await _authService.RegisterAsync(usuarioDto);
                
                return CreatedAtAction(
                    nameof(Register), 
                    new { id = response.IdUsuario }, 
                    response
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registro falhou: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar registro");
                return StatusCode(500, new { message = "Erro interno ao processar registro" });
            }
        }
    }
}
