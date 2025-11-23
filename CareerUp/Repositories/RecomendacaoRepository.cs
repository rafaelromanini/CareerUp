using CareerUp.Data;
using CareerUp.Models;
using CareerUp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerUp.Repositories;

/// <summary>
/// Repositório para acesso aos dados de recomendações.
/// </summary>
public class RecomendacaoRepository : IRecomendacaoRepository
{
    private readonly OracleDbContext _context;

    public RecomendacaoRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<Recomendacao> AddAsync(Recomendacao recomendacao)
    {
        await _context.Recomendacoes.AddAsync(recomendacao);
        await _context.SaveChangesAsync();
        
        // Reload para incluir navegação do Usuario
        await _context.Entry(recomendacao)
            .Reference(r => r.Usuario)
            .LoadAsync();
        
        return recomendacao;
    }

    public async Task<Recomendacao?> GetByIdAsync(long idRecomendacao)
    {
        return await _context.Recomendacoes
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.IdRecomendacao == idRecomendacao);
    }

    public async Task<(List<Recomendacao> items, int totalCount)> GetByUsuarioIdAsync(long idUsuario, int pageNumber, int pageSize)
    {
        var query = _context.Recomendacoes
            .Include(r => r.Usuario)
            .Where(r => r.IdUsuario == idUsuario)
            .OrderByDescending(r => r.DataGeracao);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Recomendacao> items, int totalCount)> GetByUsuarioIdAndMonthAsync(long idUsuario, int mes, int pageNumber, int pageSize)
    {
        var query = _context.Recomendacoes
            .Include(r => r.Usuario)
            .Where(r => r.IdUsuario == idUsuario && r.DataGeracao.Month == mes)
            .OrderByDescending(r => r.DataGeracao);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> DeleteAsync(long idRecomendacao)
    {
        var recomendacao = await _context.Recomendacoes.FindAsync(idRecomendacao);
        
        if (recomendacao == null)
            return false;

        _context.Recomendacoes.Remove(recomendacao);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(long idRecomendacao)
    {
        return await _context.Recomendacoes.CountAsync(r => r.IdRecomendacao == idRecomendacao) > 0;
    }
}
