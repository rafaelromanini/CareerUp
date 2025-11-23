using CareerUp.Helpers;
using CareerUp.Models.DTOs.Common;
using CareerUp.Models.DTOs.Usuario;
using CareerUp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerUp.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de usuários
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os usuários com paginação (apenas GERENTE)
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 5)</param>
        /// <returns>Lista paginada de usuários</returns>
        /// <response code="200">Lista retornada com sucesso</response>
        /// <response code="403">Acesso negado - apenas gerentes</response>
        [HttpGet]
        [Authorize(Roles = "GERENTE")]
        [ProducesResponseType(typeof(PagedResponseDto<UsuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResponseDto<UsuarioResponseDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            try
            {
                _logger.LogInformation("Listando usuários - Página: {PageNumber}", pageNumber);

                var (usuarios, totalCount) = await _usuarioService.GetAllAsync(pageNumber, pageSize);
                
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var response = new PagedResponseDto<UsuarioResponseDto>
                {
                    Data = usuarios.ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalRecords = totalCount
                };

                // Adiciona links HATEOAS de paginação
                var baseUrl = HateoasLinks.GetBaseUrl(Request, "/api/v1/usuarios");
                HateoasLinks.AddPaginationLinks(response, baseUrl, pageNumber, pageSize);

                // Adiciona links para cada usuário
                foreach (var usuario in response.Data)
                {
                    HateoasLinks.AddUsuarioLinks(usuario.Links, usuario.IdUsuario, baseUrl);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários");
                return StatusCode(500, new { message = "Erro interno ao listar usuários" });
            }
        }

        /// <summary>
        /// Obtém dados do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponseDto>> GetMe()
        {
            try
            {
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                _logger.LogInformation("Buscando dados do usuário autenticado: {UserId}", userId);

                var usuario = await _usuarioService.GetByIdAsync(userId);
                
                var baseUrl = HateoasLinks.GetBaseUrl(Request, "/api/v1/usuarios");
                HateoasLinks.AddUsuarioLinks(usuario.Links, usuario.IdUsuario, baseUrl);

                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário autenticado");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        /// <summary>
        /// Obtém usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponseDto>> GetById(long id)
        {
            try
            {
                _logger.LogInformation("Buscando usuário por ID: {IdUsuario}", id);

                // Verifica autorização: usuário comum só pode ver seus próprios dados
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userRole != "GERENTE" && userId != id)
                {
                    _logger.LogWarning("Acesso negado para usuário {UserId} tentando acessar {Id}", userId, id);
                    return Forbid();
                }

                var usuario = await _usuarioService.GetByIdAsync(id);
                
                var baseUrl = HateoasLinks.GetBaseUrl(Request, "/api/v1/usuarios");
                HateoasLinks.AddUsuarioLinks(usuario.Links, usuario.IdUsuario, baseUrl);

                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        /// <summary>
        /// Atualiza cargo do usuário (apenas GERENTE)
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="cargoDto">Novo cargo</param>
        /// <returns>Dados atualizados do usuário</returns>
        /// <response code="200">Cargo atualizado com sucesso</response>
        /// <response code="403">Acesso negado - apenas gerentes</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id}/cargo")]
        [Authorize(Roles = "GERENTE")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponseDto>> UpdateCargo(
            long id, 
            [FromBody] AtualizarCargoDto cargoDto)
        {
            try
            {
                _logger.LogInformation("Atualizando cargo do usuário: {IdUsuario}", id);

                var usuario = await _usuarioService.UpdateCargoAsync(id, cargoDto.Cargo);
                
                var baseUrl = HateoasLinks.GetBaseUrl(Request, "/api/v1/usuarios");
                HateoasLinks.AddUsuarioLinks(usuario.Links, usuario.IdUsuario, baseUrl);

                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cargo");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        /// <summary>
        /// Atualiza habilidades do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="habilidadesDto">Novas habilidades</param>
        /// <returns>Dados atualizados do usuário</returns>
        /// <response code="200">Habilidades atualizadas com sucesso</response>
        /// <response code="403">Acesso negado - apenas o próprio usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id}/habilidades")]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioResponseDto>> UpdateHabilidades(
            long id,
            [FromBody] HabilidadeRequestDto habilidadesDto)
        {
            try
            {
                // Verifica autorização: usuário só pode atualizar suas próprias habilidades
                var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId != id)
                {
                    _logger.LogWarning("Acesso negado: usuário {UserId} tentando atualizar {Id}", userId, id);
                    return Forbid();
                }

                _logger.LogInformation("Atualizando habilidades do usuário: {IdUsuario}", id);

                var usuario = await _usuarioService.UpdateHabilidadesAsync(id, habilidadesDto);
                
                var baseUrl = HateoasLinks.GetBaseUrl(Request, "/api/v1/usuarios");
                HateoasLinks.AddUsuarioLinks(usuario.Links, usuario.IdUsuario, baseUrl);

                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar habilidades");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        /// <summary>
        /// Exclui um usuário (apenas GERENTE)
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Usuário excluído com sucesso</response>
        /// <response code="403">Acesso negado - apenas gerentes</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "GERENTE")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                _logger.LogInformation("Excluindo usuário: {IdUsuario}", id);

                await _usuarioService.DeleteAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Usuário não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }
    }
}
