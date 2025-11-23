using CareerUp.Data;
using CareerUp.Models;
using CareerUp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerUp.Repositories
{
    /// <summary>
    /// Repositório para operações de LoginUsuario no banco de dados
    /// </summary>
    public class LoginUsuarioRepository : ILoginUsuarioRepository
    {
        private readonly OracleDbContext _context;
        private readonly ILogger<LoginUsuarioRepository> _logger;

        public LoginUsuarioRepository(OracleDbContext context, ILogger<LoginUsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LoginUsuario?> GetByLoginAsync(string login)
        {
            _logger.LogInformation("Buscando login de usuário: {Login}", login);
            return await _context.LoginsUsuarios
                .Include(l => l.Usuario)
                    .ThenInclude(u => u.Habilidade)
                .FirstOrDefaultAsync(l => l.Login == login);
        }

        public async Task<LoginUsuario?> GetByUsuarioIdAsync(long usuarioId)
        {
            _logger.LogInformation("Buscando login por ID de usuário: {IdUsuario}", usuarioId);
            return await _context.LoginsUsuarios
                .FirstOrDefaultAsync(l => l.IdUsuario == usuarioId);
        }

        public async Task<bool> LoginExistsAsync(string login)
        {
            var count = await _context.LoginsUsuarios
                .Where(l => l.Login == login)
                .CountAsync();
            return count > 0;
        }
    }
}
