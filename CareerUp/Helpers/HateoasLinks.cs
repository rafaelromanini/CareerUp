using CareerUp.Models.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace CareerUp.Helpers
{
    /// <summary>
    /// Helper para criação de links HATEOAS
    /// </summary>
    public static class HateoasLinks
    {
        /// <summary>
        /// Adiciona links de paginação em um PagedResponseDto
        /// </summary>
        public static void AddPaginationLinks<T>(
            PagedResponseDto<T> response,
            string baseUrl,
            int pageNumber,
            int pageSize)
        {
            // Link para a própria página (self)
            response.Links.Add(new Link(
                "self",
                $"{baseUrl}?pageNumber={pageNumber}&pageSize={pageSize}",
                "GET"
            ));

            // Link para primeira página
            response.Links.Add(new Link(
                "first",
                $"{baseUrl}?pageNumber=1&pageSize={pageSize}",
                "GET"
            ));

            // Link para página anterior (se existir)
            if (response.HasPrevious)
            {
                response.Links.Add(new Link(
                    "previous",
                    $"{baseUrl}?pageNumber={pageNumber - 1}&pageSize={pageSize}",
                    "GET"
                ));
            }

            // Link para próxima página (se existir)
            if (response.HasNext)
            {
                response.Links.Add(new Link(
                    "next",
                    $"{baseUrl}?pageNumber={pageNumber + 1}&pageSize={pageSize}",
                    "GET"
                ));
            }

            // Link para última página
            response.Links.Add(new Link(
                "last",
                $"{baseUrl}?pageNumber={response.TotalPages}&pageSize={pageSize}",
                "GET"
            ));
        }

        /// <summary>
        /// Adiciona links HATEOAS para um recurso de usuário
        /// </summary>
        public static void AddUsuarioLinks(List<Link> links, long idUsuario, string baseUrl)
        {
            links.Add(new Link("self", $"{baseUrl}/{idUsuario}", "GET"));
            links.Add(new Link("update-cargo", $"{baseUrl}/{idUsuario}/cargo", "PUT"));
            links.Add(new Link("update-habilidades", $"{baseUrl}/{idUsuario}/habilidades", "PUT"));
            links.Add(new Link("delete", $"{baseUrl}/{idUsuario}", "DELETE"));
        }

        /// <summary>
        /// Constrói URL base a partir do request
        /// </summary>
        public static string GetBaseUrl(HttpRequest request, string path)
        {
            return $"{request.Scheme}://{request.Host}{path}";
        }

        /// <summary>
        /// Gera links HATEOAS para uma recomendação
        /// </summary>
        public static List<Link> GenerateRecomendacaoLinks(long idRecomendacao, long idUsuario, HttpContext context)
        {
            var baseUrl = GetBaseUrl(context.Request, "/api/v1/recomendacoes");
            
            var links = new List<Link>
            {
                new("self", $"{baseUrl}/{idRecomendacao}", "GET"),
                new("minhas-recomendacoes", $"{baseUrl}/minhas", "GET"),
                new("gerar-nova", $"{baseUrl}/gerar", "POST"),
                new("delete", $"{baseUrl}/{idRecomendacao}", "DELETE")
            };

            return links;
        }

        /// <summary>
        /// Gera links de paginação
        /// </summary>
        public static List<Link> GeneratePaginationLinks(
            string resourcePath,
            int pageNumber,
            int pageSize,
            int totalCount,
            HttpContext context)
        {
            var baseUrl = GetBaseUrl(context.Request, $"/api/v1/recomendacoes/{resourcePath}");
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var links = new List<Link>
            {
                new("self", $"{baseUrl}?pageNumber={pageNumber}&pageSize={pageSize}", "GET"),
                new("first", $"{baseUrl}?pageNumber=1&pageSize={pageSize}", "GET")
            };

            if (pageNumber > 1)
            {
                links.Add(new("previous", $"{baseUrl}?pageNumber={pageNumber - 1}&pageSize={pageSize}", "GET"));
            }

            if (pageNumber < totalPages)
            {
                links.Add(new("next", $"{baseUrl}?pageNumber={pageNumber + 1}&pageSize={pageSize}", "GET"));
            }

            links.Add(new("last", $"{baseUrl}?pageNumber={totalPages}&pageSize={pageSize}", "GET"));

            return links;
        }
    }
}
