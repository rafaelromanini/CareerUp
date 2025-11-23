using CareerUp.Models;
using CareerUp.Models.DTOs.Usuario;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services.Interfaces;

namespace CareerUp.Services
{
    /// <summary>
    /// Serviço de gerenciamento de usuários
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHabilidadeRepository _habilidadeRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IHabilidadeRepository habilidadeRepository,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _habilidadeRepository = habilidadeRepository;
            _logger = logger;
        }

        public async Task<UsuarioResponseDto> GetByIdAsync(long id)
        {
            _logger.LogInformation("Buscando usuário por ID: {IdUsuario}", id);

            var usuario = await _usuarioRepository.GetByIdWithDetailsAsync(id);
            
            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado: {IdUsuario}", id);
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            return UsuarioResponseDto.FromEntity(usuario);
        }

        public async Task<(IEnumerable<UsuarioResponseDto> Usuarios, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize)
        {
            _logger.LogInformation("Buscando usuários - Página: {PageNumber}, Tamanho: {PageSize}", 
                pageNumber, pageSize);

            var totalCount = await _usuarioRepository.GetCountAsync();
            var usuarios = await _usuarioRepository.GetAllWithDetailsAsync();

            // Aplica paginação
            var usuariosPaginados = usuarios
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => UsuarioResponseDto.FromEntity(u))
                .ToList();

            return (usuariosPaginados, totalCount);
        }

        public async Task<UsuarioResponseDto> UpdateCargoAsync(long id, string cargo)
        {
            _logger.LogInformation("Atualizando cargo do usuário: {IdUsuario}", id);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado para atualização: {IdUsuario}", id);
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            usuario.Cargo = cargo;
            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Cargo atualizado com sucesso para usuário: {IdUsuario}", id);

            var usuarioAtualizado = await _usuarioRepository.GetByIdWithDetailsAsync(id);
            return UsuarioResponseDto.FromEntity(usuarioAtualizado!);
        }

        public async Task<UsuarioResponseDto> UpdateHabilidadesAsync(
            long id, HabilidadeRequestDto habilidadesDto)
        {
            _logger.LogInformation("Atualizando habilidades do usuário: {IdUsuario}", id);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado para atualização: {IdUsuario}", id);
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            var habilidade = await _habilidadeRepository.GetByUsuarioIdAsync(id);
            
            if (habilidade == null)
            {
                // Cria novas habilidades se não existirem
                habilidade = new Habilidade
                {
                    IdUsuario = id,
                    HabilidadePrimaria = habilidadesDto.HabilidadePrimaria,
                    HabilidadeSecundaria = habilidadesDto.HabilidadeSecundaria,
                    HabilidadeTerciaria = habilidadesDto.HabilidadeTerciaria
                };
                await _habilidadeRepository.CreateAsync(habilidade);
            }
            else
            {
                // Atualiza habilidades existentes
                habilidade.HabilidadePrimaria = habilidadesDto.HabilidadePrimaria;
                habilidade.HabilidadeSecundaria = habilidadesDto.HabilidadeSecundaria;
                habilidade.HabilidadeTerciaria = habilidadesDto.HabilidadeTerciaria;
                await _habilidadeRepository.UpdateAsync(habilidade);
            }

            _logger.LogInformation("Habilidades atualizadas com sucesso para usuário: {IdUsuario}", id);

            var usuarioAtualizado = await _usuarioRepository.GetByIdWithDetailsAsync(id);
            return UsuarioResponseDto.FromEntity(usuarioAtualizado!);
        }

        public async Task DeleteAsync(long id)
        {
            _logger.LogInformation("Excluindo usuário: {IdUsuario}", id);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado para exclusão: {IdUsuario}", id);
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            await _usuarioRepository.DeleteAsync(id);
            
            _logger.LogInformation("Usuário excluído com sucesso: {IdUsuario}", id);
        }
    }
}
