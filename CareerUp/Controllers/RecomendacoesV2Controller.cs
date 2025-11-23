using CareerUp.Helpers;
using CareerUp.Models.DTOs.Common;
using CareerUp.Models.DTOs.Recomendacao;
using CareerUp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Versioning;

namespace CareerUp.Controllers;

/// <summary>
/// Controller v2 para gerenciamento de recomendações de carreira com filtro por mês.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/recomendacoes")]
[ApiVersion("2.0")]
[Authorize]
[Produces("application/json")]
public class RecomendacoesV2Controller : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;
    private readonly ILogger<RecomendacoesV2Controller> _logger;

    public RecomendacoesV2Controller(
        IRecomendacaoService recomendacaoService,
        ILogger<RecomendacoesV2Controller> logger)
    {
        _recomendacaoService = recomendacaoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista recomendações do usuário autenticado com filtro por mês e paginação.
    /// </summary>
    /// <param name="mes">Mês para filtrar (1-12). Opcional.</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 5)</param>
    /// <returns>Lista paginada de recomendações</returns>
    /// <response code="200">Lista de recomendações</response>
    /// <response code="400">Mês inválido</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("minhas")]
    [ProducesResponseType(typeof(PagedResponseDto<RecomendacaoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponseDto<RecomendacaoResponseDto>>> GetMinhasRecomendacoes(
        [FromQuery] int? mes = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var idUsuario = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            List<RecomendacaoResponseDto> items;
            int totalCount;

            if (mes.HasValue)
            {
                // Filtrar por mês
                (items, totalCount) = await _recomendacaoService.GetByUsuarioIdAndMonthAsync(
                    idUsuario, idUsuario, mes.Value, pageNumber, pageSize);
            }
            else
            {
                // Sem filtro de mês (comportamento padrão)
                (items, totalCount) = await _recomendacaoService.GetByUsuarioIdAsync(
                    idUsuario, idUsuario, pageNumber, pageSize);
            }

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
                    mes.HasValue ? $"minhas?mes={mes.Value}" : "minhas",
                    pageNumber,
                    pageSize,
                    totalCount,
                    HttpContext)
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Mês inválido fornecido: {Mes}", mes);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar recomendações do usuário");
            return StatusCode(500, new { message = "Erro interno ao listar recomendações" });
        }
    }
}
