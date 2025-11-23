using CareerUp.Helpers;
using CareerUp.Models.DTOs.Common;
using CareerUp.Models.DTOs.Recomendacao;
using CareerUp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerUp.Controllers;

/// <summary>
/// Controller para gerenciamento de recomendações de carreira.
/// </summary>
[ApiController]
[Route("api/v1/recomendacoes")]
[Authorize]
[Produces("application/json")]
public class RecomendacoesController : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;
    private readonly ILogger<RecomendacoesController> _logger;

    public RecomendacoesController(
        IRecomendacaoService recomendacaoService,
        ILogger<RecomendacoesController> logger)
    {
        _recomendacaoService = recomendacaoService;
        _logger = logger;
    }

    /// <summary>
    /// Gera uma nova recomendação de carreira usando ML.NET.
    /// </summary>
    /// <returns>Recomendação gerada</returns>
    /// <response code="201">Recomendação criada com sucesso</response>
    /// <response code="400">Erro de validação</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("gerar")]
    [ProducesResponseType(typeof(RecomendacaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RecomendacaoResponseDto>> GerarRecomendacao()
    {
        try
        {
            var idUsuario = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var recomendacao = await _recomendacaoService.GerarRecomendacaoAsync(idUsuario);

            // Adicionar links HATEOAS
            recomendacao.Links = HateoasLinks.GenerateRecomendacaoLinks(
                recomendacao.IdRecomendacao,
                recomendacao.IdUsuario,
                HttpContext);

            return CreatedAtAction(
                nameof(GetById),
                new { id = recomendacao.IdRecomendacao },
                recomendacao);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Usuário não encontrado ao gerar recomendação");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao gerar recomendação");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar recomendação");
            return StatusCode(500, new { message = "Erro interno ao gerar recomendação" });
        }
    }

    /// <summary>
    /// Busca uma recomendação por ID.
    /// </summary>
    /// <param name="id">ID da recomendação</param>
    /// <returns>Dados da recomendação</returns>
    /// <response code="200">Recomendação encontrada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Sem permissão</response>
    /// <response code="404">Recomendação não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RecomendacaoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecomendacaoResponseDto>> GetById(long id)
    {
        try
        {
            var idUsuario = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var recomendacao = await _recomendacaoService.GetByIdAsync(id, idUsuario);

            if (recomendacao == null)
                return NotFound(new { message = "Recomendação não encontrada" });

            // Adicionar links HATEOAS
            recomendacao.Links = HateoasLinks.GenerateRecomendacaoLinks(
                recomendacao.IdRecomendacao,
                recomendacao.IdUsuario,
                HttpContext);

            return Ok(recomendacao);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso negado à recomendação ID={Id}", id);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar recomendação ID={Id}", id);
            return StatusCode(500, new { message = "Erro interno ao buscar recomendação" });
        }
    }

    /// <summary>
    /// Lista recomendações do usuário autenticado com paginação.
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 5)</param>
    /// <returns>Lista paginada de recomendações</returns>
    /// <response code="200">Lista de recomendações</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("minhas")]
    [ProducesResponseType(typeof(PagedResponseDto<RecomendacaoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponseDto<RecomendacaoResponseDto>>> GetMinhasRecomendacoes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var idUsuario = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (items, totalCount) = await _recomendacaoService.GetByUsuarioIdAsync(
                idUsuario, idUsuario, pageNumber, pageSize);

            // Adicionar links HATEOAS para cada item
            foreach (var item in items)
            {
                item.Links = HateoasLinks.GenerateRecomendacaoLinks(
                    item.IdRecomendacao,
                    item.IdUsuario,
                    HttpContext);
            }

            var response = new PagedResponseDto<RecomendacaoResponseDto>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                Links = HateoasLinks.GeneratePaginationLinks(
                    "minhas",
                    pageNumber,
                    pageSize,
                    totalCount,
                    HttpContext)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar recomendações do usuário");
            return StatusCode(500, new { message = "Erro interno ao listar recomendações" });
        }
    }

    /// <summary>
    /// Lista recomendações de um usuário específico (apenas GERENTE).
    /// </summary>
    /// <param name="idUsuario">ID do usuário</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 5)</param>
    /// <returns>Lista paginada de recomendações</returns>
    /// <response code="200">Lista de recomendações</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Sem permissão (não é gerente)</response>
    [HttpGet("usuario/{idUsuario}")]
    [Authorize(Roles = "GERENTE")]
    [ProducesResponseType(typeof(PagedResponseDto<RecomendacaoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResponseDto<RecomendacaoResponseDto>>> GetByUsuarioId(
        long idUsuario,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var idUsuarioAutenticado = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (items, totalCount) = await _recomendacaoService.GetByUsuarioIdAsync(
                idUsuario, idUsuarioAutenticado, pageNumber, pageSize);

            // Adicionar links HATEOAS
            foreach (var item in items)
            {
                item.Links = HateoasLinks.GenerateRecomendacaoLinks(
                    item.IdRecomendacao,
                    item.IdUsuario,
                    HttpContext);
            }

            var response = new PagedResponseDto<RecomendacaoResponseDto>
            {
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                Links = HateoasLinks.GeneratePaginationLinks(
                    $"usuario/{idUsuario}",
                    pageNumber,
                    pageSize,
                    totalCount,
                    HttpContext)
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso negado às recomendações do usuário ID={IdUsuario}", idUsuario);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar recomendações do usuário ID={IdUsuario}", idUsuario);
            return StatusCode(500, new { message = "Erro interno ao listar recomendações" });
        }
    }

    /// <summary>
    /// Exclui uma recomendação.
    /// </summary>
    /// <param name="id">ID da recomendação</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Recomendação excluída com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Sem permissão</response>
    /// <response code="404">Recomendação não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var idUsuario = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var deleted = await _recomendacaoService.DeleteAsync(id, idUsuario);

            if (!deleted)
                return NotFound(new { message = "Recomendação não encontrada" });

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acesso negado ao excluir recomendação ID={Id}", id);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir recomendação ID={Id}", id);
            return StatusCode(500, new { message = "Erro interno ao excluir recomendação" });
        }
    }
}
