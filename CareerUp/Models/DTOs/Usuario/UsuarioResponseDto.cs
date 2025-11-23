using CareerUp.Models.Enums;
using CareerUp.Models.DTOs.Common;

namespace CareerUp.Models.DTOs.Usuario
{
    /// <summary>
    /// DTO para resposta com dados do usuário
    /// </summary>
    public class UsuarioResponseDto
    {
        /// <summary>
        /// ID do usuário
        /// </summary>
        public long IdUsuario { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário
        /// </summary>
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Cargo atual do usuário
        /// </summary>
        public string Cargo { get; set; } = string.Empty;

        /// <summary>
        /// Papel do usuário no sistema
        /// </summary>
        public string Papel { get; set; } = string.Empty;

        /// <summary>
        /// Login do usuário
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Habilidade primária
        /// </summary>
        public string? HabilidadePrimaria { get; set; }

        /// <summary>
        /// Habilidade secundária
        /// </summary>
        public string? HabilidadeSecundaria { get; set; }

        /// <summary>
        /// Habilidade terciária
        /// </summary>
        public string? HabilidadeTerciaria { get; set; }

        /// <summary>
        /// Links HATEOAS
        /// </summary>
        public List<Link> Links { get; set; } = new();

        /// <summary>
        /// Converte entidade Usuario para DTO
        /// </summary>
        public static UsuarioResponseDto FromEntity(Models.Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                Cpf = usuario.Cpf,
                Email = usuario.Email,
                Cargo = usuario.Cargo,
                Papel = usuario.Papel.ToString(),
                Login = usuario.LoginUsuario?.Login ?? string.Empty,
                HabilidadePrimaria = usuario.Habilidade?.HabilidadePrimaria,
                HabilidadeSecundaria = usuario.Habilidade?.HabilidadeSecundaria,
                HabilidadeTerciaria = usuario.Habilidade?.HabilidadeTerciaria
            };
        }
    }
}
