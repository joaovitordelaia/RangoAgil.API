using RangoAgil.API.EndPointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndPoints = c.MapGroup("/rangos");
        var rangosComIdEndPoints = rangosEndPoints.MapGroup("/{rangoId:int}");

        // O endpoint retorna um Task de Results que pode ser NoContent ou Ok com uma coleção de RangoDTO
        rangosEndPoints.MapGet("", RangoHandlers.GetRangoNomeAsync);

        // O endpoint retorna um Task de Results que pode ser NoContent ou Ok com um objeto RangoDTO
        rangosComIdEndPoints.MapGet("", RangoHandlers.GetRangoIdAsync).WithName("GetRangoId"); // Nomeia a rota como "GetRangoId"

        // O endpoint retorna um Task de CreatedAtRoute com um objeto RangoDTO
        rangosEndPoints.MapPost("", RangoHandlers.PostRangoAsync);

        // O endpoint aceita um parâmetro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou Ok
        rangosComIdEndPoints.MapPut("", RangoHandlers.PutRangoAsync);

        // O endpoint aceita um parâmetro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou NoContent
        rangosComIdEndPoints.MapDelete("", RangoHandlers.DeleteRangoAsync);
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosIdIngredEndPoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");

        // Mapeia uma rota GET para o endpoint "/rangos/{rangoId:int}/ingredientes"
        rangosIdIngredEndPoints.MapGet("", IngredienteHandlers.getIngredienteAsync);
    }
}

