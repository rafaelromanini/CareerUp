using CareerUp.Models;
using CareerUp.Models.DTOs.Auth;
using CareerUp.Models.DTOs.Usuario;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace CareerUp.Services
{
    /// <summary>
    /// Serviço de autenticação e registro de usuários
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILoginUsuarioRepository _loginRepository;
        private readonly IHabilidadeRepository _habilidadeRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            ILoginUsuarioRepository loginRepository,
            IHabilidadeRepository habilidadeRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _loginRepository = loginRepository;
            _habilidadeRepository = habilidadeRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            _logger.LogInformation("Tentativa de login para usuário: {Login}", loginDto.Login);

            // Busca usuário por login
            var loginUsuario = await _loginRepository.GetByLoginAsync(loginDto.Login);
            
            if (loginUsuario == null)
            {
                _logger.LogWarning("Login não encontrado: {Login}", loginDto.Login);
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            // Valida senha
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Senha, loginUsuario.Senha))
            {
                _logger.LogWarning("Senha inválida para login: {Login}", loginDto.Login);
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            // Gera token JWT
            var token = GenerateJwtToken(loginUsuario.Usuario);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]));

            _logger.LogInformation("Login realizado com sucesso para: {Login}", loginDto.Login);

            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                Usuario = UsuarioResponseDto.FromEntity(loginUsuario.Usuario)
            };
        }

        public async Task<UsuarioResponseDto> RegisterAsync(UsuarioRequestDto usuarioDto)
        {
            _logger.LogInformation("Iniciando registro de novo usuário: {NomeUsuario}", usuarioDto.NomeUsuario);

            // Validações de unicidade
            if (await _usuarioRepository.GetByCpfAsync(usuarioDto.Cpf) != null)
            {
                _logger.LogWarning("CPF já cadastrado: {Cpf}", usuarioDto.Cpf);
                throw new InvalidOperationException("CPF já cadastrado");
            }

            if (await _usuarioRepository.GetByEmailAsync(usuarioDto.Email) != null)
            {
                _logger.LogWarning("Email já cadastrado: {Email}", usuarioDto.Email);
                throw new InvalidOperationException("Email já cadastrado");
            }

            if (await _loginRepository.LoginExistsAsync(usuarioDto.LoginUsuario.Login))
            {
                _logger.LogWarning("Login já existe: {Login}", usuarioDto.LoginUsuario.Login);
                throw new InvalidOperationException("Login já existe");
            }

            // Cria entidade Usuario
            var usuario = new Usuario
            {
                NomeUsuario = usuarioDto.NomeUsuario,
                Cpf = usuarioDto.Cpf,
                Email = usuarioDto.Email,
                Cargo = usuarioDto.Cargo,
                Papel = usuarioDto.Papel
            };

            // Cria LoginUsuario com senha criptografada
            usuario.LoginUsuario = new LoginUsuario
            {
                Login = usuarioDto.LoginUsuario.Login,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.LoginUsuario.Senha, workFactor: 12)
            };

            // Cria Habilidades
            usuario.Habilidade = new Habilidade
            {
                HabilidadePrimaria = usuarioDto.Habilidades.HabilidadePrimaria,
                HabilidadeSecundaria = usuarioDto.Habilidades.HabilidadeSecundaria,
                HabilidadeTerciaria = usuarioDto.Habilidades.HabilidadeTerciaria
            };

            // Salva no banco
            var usuarioCriado = await _usuarioRepository.CreateAsync(usuario);

            _logger.LogInformation("Usuário registrado com sucesso: {IdUsuario}", usuarioCriado.IdUsuario);

            // Busca usuário com detalhes para retornar
            var usuarioCompleto = await _usuarioRepository.GetByIdWithDetailsAsync(usuarioCriado.IdUsuario);
            
            return UsuarioResponseDto.FromEntity(usuarioCompleto!);
        }

        public async Task<bool> ValidateCredentialsAsync(string login, string senha)
        {
            var loginUsuario = await _loginRepository.GetByLoginAsync(login);
            
            if (loginUsuario == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(senha, loginUsuario.Senha);
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Papel.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
