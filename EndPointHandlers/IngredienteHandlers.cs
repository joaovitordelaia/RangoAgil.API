using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPointHandlers;

public static class IngredienteHandlers
{
    public static async Task<Results<NotFound, Ok<IEnumerable<IngredienteDTO>>>> getIngredienteAsync(
        RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId
    )
    {
        // Procura a entidade "Rango" pelo seu ID.
        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        // Se não encontrar a entidade "Rango", retorna um resultado "NotFound".
        if (rangoEntity == null)
        {
            return TypedResults.NotFound();
        }
        // Inclui os ingredientes associados ao "Rango" e busca novamente a entidade pelo ID.
        var rango = await rangoDbContext.Rangos
            .Include(rango => rango.Ingredientes)
            .FirstOrDefaultAsync(rango => rango.Id == rangoId);

        // Mapeia a lista de ingredientes para DTOs.
        var ingredientesDTO = mapper.Map<IEnumerable<IngredienteDTO>>(rango?.Ingredientes);

        // Retorna um resultado "Ok" com a lista de ingredientes mapeados.
        return TypedResults.Ok(ingredientesDTO);
    }
}

