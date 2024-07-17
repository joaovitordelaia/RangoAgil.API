using RangoAgil.API.EndPointFilters;
using RangoAgil.API.EndPointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var rangosEndPoints = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndPoints = rangosEndPoints.MapGroup("/{rangoId:int}");

        // importante criar um group novo para inserir os filtro, se usar uma variavel ela sera alterada de forma global tambem
        var rangosComIdAndLockedEndPoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
                                                                .AddEndpointFilter(new RangoLocked(10))
                                                                .AddEndpointFilter(new RangoLocked(5));

        // O endpoint retorna um Task de Results que pode ser NoContent ou Ok com uma coleção de RangoDTO
        rangosEndPoints.MapGet("", RangoHandlers.GetRangoNomeAsync);


        // O endpoint retorna um Task de Results que pode ser NoContent ou Ok com um objeto RangoDTO
        rangosComIdEndPoints.MapGet("", RangoHandlers.GetRangoIdAsync).WithName("GetRangoId"); // Nomeia a rota como "GetRangoId"

        // O endpoint retorna um Task de CreatedAtRoute com um objeto RangoDTO
        rangosEndPoints.MapPost("", RangoHandlers.PostRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>();

        // O endpoint aceita um parâmetro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou Ok
        rangosComIdAndLockedEndPoints.MapPut("", RangoHandlers.PutRangoAsync);

        // O endpoint aceita um parâmetro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou NoContent
        rangosComIdAndLockedEndPoints.MapDelete("", RangoHandlers.DeleteRangoAsync);

    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var IngredientesEndPoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");

        // Mapeia uma rota GET para o endpoint "/rangos/{rangoId:int}/ingredientes"
        IngredientesEndPoints.MapGet("", IngredienteHandlers.getIngredienteAsync);
        //IngredientesEndPoints.MapPost("", );
    }
}

