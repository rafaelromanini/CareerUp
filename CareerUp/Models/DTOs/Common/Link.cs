namespace CareerUp.Models.DTOs.Common
{
    /// <summary>
    /// Representa um link HATEOAS
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Relação do link (self, next, previous, etc)
        /// </summary>
        public string Rel { get; set; } = string.Empty;

        /// <summary>
        /// URL do recurso
        /// </summary>
        public string Href { get; set; } = string.Empty;

        /// <summary>
        /// Método HTTP (GET, POST, PUT, DELETE)
        /// </summary>
        public string Method { get; set; } = string.Empty;

        public Link(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }
}
