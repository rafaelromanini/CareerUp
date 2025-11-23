using CareerUp.Data;
using CareerUp.Models;
using CareerUp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareerUp.Repositories
{
    /// <summary>
    /// Repositório para operações de Usuário no banco de dados
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly OracleDbContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(OracleDbContext context, ILogger<UsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            _logger.LogInformation("Buscando usuário por ID: {IdUsuario}", id);
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByIdWithDetailsAsync(long id)
        {
            _logger.LogInformation("Buscando usuário com detalhes por ID: {IdUsuario}", id);
            return await _context.Usuarios
                .Include(u => u.LoginUsuario)
                .Include(u => u.Habilidade)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<Usuario?> GetByCpfAsync(string cpf)
        {
            _logger.LogInformation("Buscando usuário por CPF");
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cpf == cpf);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Buscando usuário por Email");
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todos os usuários");
            return await _context.Usuarios
                .OrderBy(u => u.NomeUsuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAllWithDetailsAsync()
        {
            _logger.LogInformation("Buscando todos os usuários com detalhes");
            return await _context.Usuarios
                .Include(u => u.LoginUsuario)
                .Include(u => u.Habilidade)
                .OrderBy(u => u.NomeUsuario)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Usuarios.CountAsync();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _logger.LogInformation("Criando novo usuário: {NomeUsuario}", usuario.NomeUsuario);
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            _logger.LogInformation("Atualizando usuário: {IdUsuario}", usuario.IdUsuario);
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation("Removendo usuário: {IdUsuario}", id);
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(long id)
        {
            var count = await _context.Usuarios
                .Where(u => u.IdUsuario == id)
                .CountAsync();
            return count > 0;
        }
    }
}
