using CareerUp.Models;
using CareerUp.Models.DTOs.Recomendacao;
using CareerUp.Models.Enums;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services.Interfaces;

namespace CareerUp.Services;

/// <summary>
/// Serviço de lógica de negócio para recomendações.
/// </summary>
public class RecomendacaoService : IRecomendacaoService
{
    private readonly IRecomendacaoRepository _recomendacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMLPredictionService _mlPredictionService;
    private readonly ILogger<RecomendacaoService> _logger;

    public RecomendacaoService(
        IRecomendacaoRepository recomendacaoRepository,
        IUsuarioRepository usuarioRepository,
        IMLPredictionService mlPredictionService,
        ILogger<RecomendacaoService> logger)
    {
        _recomendacaoRepository = recomendacaoRepository;
        _usuarioRepository = usuarioRepository;
        _mlPredictionService = mlPredictionService;
        _logger = logger;
    }

    public async Task<RecomendacaoResponseDto> GerarRecomendacaoAsync(long idUsuario)
    {
        _logger.LogInformation("Gerando recomendação para usuário ID={IdUsuario}", idUsuario);

        // Buscar usuário com habilidades
        var usuario = await _usuarioRepository.GetByIdWithDetailsAsync(idUsuario);
        if (usuario == null)
        {
            _logger.LogWarning("Usuário ID={IdUsuario} não encontrado", idUsuario);
            throw new KeyNotFoundException("Usuário não encontrado");
        }

        if (usuario.Habilidade == null)
        {
            _logger.LogWarning("Usuário ID={IdUsuario} não possui habilidades cadastradas", idUsuario);
            throw new InvalidOperationException("Usuário não possui habilidades cadastradas");
        }

        // Gerar recomendação usando ML.NET
        var textoRecomendacao = _mlPredictionService.PredictCareerRecommendation(
            usuario.Cargo,
            usuario.Habilidade.HabilidadePrimaria,
            usuario.Habilidade.HabilidadeSecundaria,
            usuario.Habilidade.HabilidadeTerciaria
        );

        // Criar entidade de recomendação
        var recomendacao = new Recomendacao
        {
            DataGeracao = DateTime.UtcNow,
            ResultadoIa = textoRecomendacao,
            IdUsuario = idUsuario
        };

        // Salvar no banco
        recomendacao = await _recomendacaoRepository.AddAsync(recomendacao);

        _logger.LogInformation(
            "Recomendação ID={IdRecomendacao} gerada com sucesso para usuário ID={IdUsuario}",
            recomendacao.IdRecomendacao, idUsuario);

        return MapToDto(recomendacao);
    }

    public async Task<RecomendacaoResponseDto?> GetByIdAsync(long idRecomendacao, long idUsuarioAutenticado)
    {
        var recomendacao = await _recomendacaoRepository.GetByIdAsync(idRecomendacao);
        
        if (recomendacao == null)
            return null;

        // Verificar permissão (usuário só vê suas próprias recomendações, gerente vê todas)
        var usuarioAutenticado = await _usuarioRepository.GetByIdAsync(idUsuarioAutenticado);
        if (usuarioAutenticado?.Papel != PapelUsuario.GERENTE && recomendacao.IdUsuario != idUsuarioAutenticado)
        {
            _logger.LogWarning(
                "Usuário ID={IdUsuario} tentou acessar recomendação ID={IdRecomendacao} sem permissão",
                idUsuarioAutenticado, idRecomendacao);
            throw new UnauthorizedAccessException("Você não tem permissão para acessar esta recomendação");
        }

        return MapToDto(recomendacao);
    }

    public async Task<(List<RecomendacaoResponseDto> items, int totalCount)> GetByUsuarioIdAsync(
        long idUsuario,
        long idUsuarioAutenticado,
        int pageNumber,
        int pageSize)
    {
        // Verificar permissão
        var usuarioAutenticado = await _usuarioRepository.GetByIdAsync(idUsuarioAutenticado);
        if (usuarioAutenticado?.Papel != PapelUsuario.GERENTE && idUsuario != idUsuarioAutenticado)
        {
            _logger.LogWarning(
                "Usuário ID={IdUsuario} tentou listar recomendações do usuário ID={IdTarget} sem permissão",
                idUsuarioAutenticado, idUsuario);
            throw new UnauthorizedAccessException("Você não tem permissão para acessar recomendações de outros usuários");
        }

        var (items, totalCount) = await _recomendacaoRepository.GetByUsuarioIdAsync(
            idUsuario, pageNumber, pageSize);

        var dtos = items.Select(MapToDto).ToList();

        return (dtos, totalCount);
    }

    public async Task<(List<RecomendacaoResponseDto> items, int totalCount)> GetByUsuarioIdAndMonthAsync(
        long idUsuario,
        long idUsuarioAutenticado,
        int mes,
        int pageNumber,
        int pageSize)
    {
        // Verificar permissão
        var usuarioAutenticado = await _usuarioRepository.GetByIdAsync(idUsuarioAutenticado);
        if (usuarioAutenticado?.Papel != PapelUsuario.GERENTE && idUsuario != idUsuarioAutenticado)
        {
            _logger.LogWarning(
                "Usuário ID={IdUsuario} tentou listar recomendações do usuário ID={IdTarget} sem permissão",
                idUsuarioAutenticado, idUsuario);
            throw new UnauthorizedAccessException("Você não tem permissão para acessar recomendações de outros usuários");
        }

        // Validar mês
        if (mes < 1 || mes > 12)
        {
            _logger.LogWarning("Mês inválido: {Mes}", mes);
            throw new ArgumentException("Mês deve estar entre 1 e 12", nameof(mes));
        }

        var (items, totalCount) = await _recomendacaoRepository.GetByUsuarioIdAndMonthAsync(
            idUsuario, mes, pageNumber, pageSize);

        var dtos = items.Select(MapToDto).ToList();

        return (dtos, totalCount);
    }

    public async Task<bool> DeleteAsync(long idRecomendacao, long idUsuarioAutenticado)
    {
        var recomendacao = await _recomendacaoRepository.GetByIdAsync(idRecomendacao);
        
        if (recomendacao == null)
            return false;

        // Verificar permissão (usuário deleta suas recomendações, gerente deleta todas)
        var usuarioAutenticado = await _usuarioRepository.GetByIdAsync(idUsuarioAutenticado);
        if (usuarioAutenticado?.Papel != PapelUsuario.GERENTE && recomendacao.IdUsuario != idUsuarioAutenticado)
        {
            _logger.LogWarning(
                "Usuário ID={IdUsuario} tentou excluir recomendação ID={IdRecomendacao} sem permissão",
                idUsuarioAutenticado, idRecomendacao);
            throw new UnauthorizedAccessException("Você não tem permissão para excluir esta recomendação");
        }

        var deleted = await _recomendacaoRepository.DeleteAsync(idRecomendacao);
        
        if (deleted)
        {
            _logger.LogInformation(
                "Recomendação ID={IdRecomendacao} excluída por usuário ID={IdUsuario}",
                idRecomendacao, idUsuarioAutenticado);
        }

        return deleted;
    }

    private RecomendacaoResponseDto MapToDto(Recomendacao recomendacao)
    {
        return new RecomendacaoResponseDto
        {
            IdRecomendacao = recomendacao.IdRecomendacao,
            DataGeracao = recomendacao.DataGeracao,
            ResultadoIa = recomendacao.ResultadoIa,
            IdUsuario = recomendacao.IdUsuario,
            NomeUsuario = recomendacao.Usuario?.NomeUsuario ?? string.Empty,
            Cargo = recomendacao.Usuario?.Cargo ?? string.Empty,
            Links = new()
        };
    }
}
