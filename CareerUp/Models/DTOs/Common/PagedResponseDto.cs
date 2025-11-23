namespace CareerUp.Models.DTOs.Common
{
    /// <summary>
    /// DTO genérico para respostas paginadas
    /// </summary>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// Lista de dados da página atual
        /// </summary>
        public List<T> Data { get; set; } = new();

        /// <summary>
        /// Número da página atual (base 1)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de páginas
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total de registros
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Indica se há página anterior
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Indica se há próxima página
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// Links de navegação HATEOAS
        /// </summary>
        public List<Link> Links { get; set; } = new();
    }
}
