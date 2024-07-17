
using System.Net;

namespace RangoAgil.API.EndPointFilters
{
    // Define uma classe chamada LogNotFoundResponseFilter que implementa a interface IEndpointFilter
    public class LogNotFoundResponseFilter(ILogger<LogNotFoundResponseFilter> logger) : IEndpointFilter {
        // Declara um campo readonly para armazenar uma instância de ILogger para esta classe
        public readonly ILogger<LogNotFoundResponseFilter> _logger = logger;

        // Método obrigatório da interface IEndpointFilter que será chamado para processar a requisição
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var result = await next(context); // Chama o próximo delegado no pipeline de filtros e armazena o resultado

            // Verifica se o resultado é um INestedHttpResult
            // Se for, extrai o Result interno, caso contrário, usa o resultado diretamente como IResult
            var actualResult = (result is INestedHttpResult result1) ? result1.Result : (IResult)result;

            // Verifica se o resultado é um IStatusCodeHttpResult e se o código de status é 404 (NotFound)
            if (actualResult is IStatusCodeHttpResult { StatusCode: (int)HttpStatusCode.NotFound })
            {
                // Loga uma mensagem de informação informando que o recurso não foi encontrado
                _logger.LogInformation($"Resource {context.HttpContext.Request.Path} was not found");
            }

            return result; // Retorna o resultado original
        }
    }

}
