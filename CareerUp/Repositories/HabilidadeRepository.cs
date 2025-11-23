using CareerUp.Data;
using CareerUp.Models;
using CareerUp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerUp.Repositories
{
    /// <summary>
    /// Repositório para operações de Habilidade no banco de dados
    /// </summary>
    public class HabilidadeRepository : IHabilidadeRepository
    {
        private readonly OracleDbContext _context;
        private readonly ILogger<HabilidadeRepository> _logger;

        public HabilidadeRepository(OracleDbContext context, ILogger<HabilidadeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Habilidade?> GetByUsuarioIdAsync(long usuarioId)
        {
            _logger.LogInformation("Buscando habilidades do usuário: {IdUsuario}", usuarioId);
            return await _context.Habilidades
                .FirstOrDefaultAsync(h => h.IdUsuario == usuarioId);
        }

        public async Task<Habilidade> CreateAsync(Habilidade habilidade)
        {
            _logger.LogInformation("Criando habilidades para usuário: {IdUsuario}", habilidade.IdUsuario);
            await _context.Habilidades.AddAsync(habilidade);
            await _context.SaveChangesAsync();
            return habilidade;
        }

        public async Task<Habilidade> UpdateAsync(Habilidade habilidade)
        {
            _logger.LogInformation("Atualizando habilidades do usuário: {IdUsuario}", habilidade.IdUsuario);
            _context.Habilidades.Update(habilidade);
            await _context.SaveChangesAsync();
            return habilidade;
        }
    }
}
