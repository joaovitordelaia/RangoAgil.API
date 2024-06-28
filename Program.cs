using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> (RangoDbContext rangoDbContext, IMapper mapper, [FromQuery(Name = "name")] string? nomeRango) =>{

        var rangoEntity = await rangoDbContext.Rangos
                                   .Where(x => nomeRango == null ||  x.Nome.ToLower().Contains(nomeRango.ToLower()))
                                   .ToListAsync();

    if (rangoEntity == null || rangoEntity.Count <= 0)
        {
            return TypedResults.NoContent();
        }
        
    var rangoEntiDto = mapper.Map<IEnumerable<RangoDTO>>(rangoEntity);
 
        return TypedResults.Ok(rangoEntiDto);
});















app.MapGet("/rangos/{rangoId:int}/ingredientes", async (RangoDbContext rangoDbContext, IMapper mapper, int rangoId) =>
{



    return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                               .Include(rango => rango.Ingredientes)
                               .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);

});


app.MapGet("/rangos/{id:int}", async Task<Results<NoContent,Ok<RangoDTO>>> (RangoDbContext rangoDbContext, IMapper mapper, int id) =>
{

    var rango = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);// primeiro: atribuir a um var
    if (rango == null)// eliminar se houver vazio
    {
        return TypedResults.NoContent();// caso o que for procurado for null, ele retorna a resposta 204
    }
    var retornoRango = mapper.Map<RangoDTO>(rango);// se tiver, uma nova variável de transferência de dados será criada
    //chamada retornoRango
    return TypedResults.Ok(retornoRango);// e ela será entregue ao solicitante

});

app.Run();
