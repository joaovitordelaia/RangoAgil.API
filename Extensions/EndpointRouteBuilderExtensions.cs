using Microsoft.AspNetCore.Identity;
using RangoAgil.API.EndPointFilters;
using RangoAgil.API.EndPointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGroup("/identity/").MapIdentityApi<IdentityUser>();

        endpointRouteBuilder.MapGet("/pratos/{pratoid:int}", (int pratoid) => $"O prato {pratoid} é delicioso!")
           .WithOpenApi(operation =>
           {
               operation.Deprecated = true;
               return operation;
           })
           .WithSummary("vai acabar esse carai, muda logo pow");

        var rangosEndPoints = endpointRouteBuilder.MapGroup("/rangos").RequireAuthorization();
        var rangosComIdEndPoints = rangosEndPoints.MapGroup("/{rangoId:int}");
        var rangosComIdAndLockedEndPoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
            .RequireAuthorization("RequireAdminFromBrazil")
            .RequireAuthorization()
            .AddEndpointFilter(new RangoLocked(10))
            .AddEndpointFilter(new RangoLocked(5));



        rangosEndPoints.MapGet("", RangoHandlers.GetRangoNomeAsync)
            .WithOpenApi()
            .WithSummary("Retorna todos os Rangos");

        rangosComIdEndPoints.MapGet("", RangoHandlers.GetRangoIdAsync)
            .WithName("GetRangoId")
            .WithOpenApi()
            .WithSummary("Retorna um rango pela id informada")
            .AllowAnonymous();

        rangosEndPoints.MapPost("", RangoHandlers.PostRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>()
            .WithOpenApi()
            .WithSummary("Cria um novo rango no banco de dados");

        rangosComIdAndLockedEndPoints.MapPut("", RangoHandlers.PutRangoAsync)
            .WithOpenApi()
            .WithSummary("Atualiza um rango no banco de dados");

        rangosComIdAndLockedEndPoints.MapDelete("", RangoHandlers.DeleteRangoAsync)
            .WithOpenApi()
            .WithSummary("Deleta um rango no banco de dados");
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var IngredientesEndPoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes").RequireAuthorization();
        IngredientesEndPoints.MapGet("", IngredienteHandlers.getIngredienteAsync);
    }
}
