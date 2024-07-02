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

var rangosEndPoints = app.MapGroup("/rangos");
var rangosComIdEndPoints = rangosEndPoints.MapGroup("/{rangoId:int}");

// O endpoint retorna um Task de Results que pode ser NoContent ou Ok com uma cole��o de RangoDTO
rangosEndPoints.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> (
    RangoDbContext rangoDbContext,   // Injeta o contexto do banco de dados RangoDbContext
    IMapper mapper,                  // Injeta o mapeador AutoMapper
    [FromQuery(Name = "name")]       // Obt�m o par�metro de consulta 'name' opcional
    string? nomeRango                // O nome do rango a ser filtrado, pode ser nulo
) =>
{
    // Busca entidades Rango no banco de dados que correspondem ao nome fornecido (ignorando mai�sculas/min�sculas)
    // ou todos os Rangos se nomeRango for nulo
    var rangoEntity = await rangoDbContext.Rangos
        .Where(x => nomeRango == null || x.Nome.ToLower().Contains(nomeRango.ToLower()))
        .ToListAsync();

    // Verifica se a lista de entidades est� vazia
    if (rangoEntity == null || rangoEntity.Count <= 0)
    {
        // Retorna NoContent se n�o houver entidades correspondentes
        return TypedResults.NoContent();
    }

    // Mapeia a cole��o de entidades para uma cole��o de DTOs
    var rangoEntiDto = mapper.Map<IEnumerable<RangoDTO>>(rangoEntity);

    // Retorna Ok com a cole��o de DTOs
    return TypedResults.Ok(rangoEntiDto);
});


// Mapeia uma rota GET para o endpoint "/rangos/{rangoId:int}/ingredientes"
rangosComIdEndPoints.MapGet("/ingredientes", async (
    RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
    IMapper mapper,                 // Injeta o mapeador AutoMapper
    int rangoId                     // Obt�m o par�metro de rota 'rangoId' como um inteiro
) =>
{
    // Busca a entidade Rango no banco de dados com o id fornecido,
    // incluindo os Ingredientes relacionados
    var rango = await rangoDbContext.Rangos
        .Include(rango => rango.Ingredientes)
        .FirstOrDefaultAsync(rango => rango.Id == rangoId);

    // Mapeia a cole��o de Ingredientes para uma cole��o de IngredienteDTO
    var ingredientesDTO = mapper.Map<IEnumerable<IngredienteDTO>>(rango?.Ingredientes);

    // Retorna a cole��o de IngredienteDTO
    return ingredientesDTO;

});


// O endpoint retorna um Task de Results que pode ser NoContent ou Ok com um objeto RangoDTO
rangosComIdEndPoints.MapGet("", async Task<Results<NoContent, Ok<RangoDTO>>> (
    RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
    IMapper mapper,                 // Injeta o mapeador AutoMapper
    int rangoId                     // Obt�m o par�metro de rota 'rangoId' como um inteiro
) =>
{
    // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
    var rango = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    // Verifica se a entidade foi encontrada
    if (rango == null)
    {
        // Retorna NoContent se a entidade n�o existir
        return TypedResults.NoContent();
    }

    // Mapeia a entidade para um DTO de retorno
    var retornoRango = mapper.Map<RangoDTO>(rango);

    // Retorna Ok com o DTO de retorno se a entidade foi encontrada
    return TypedResults.Ok(retornoRango);

}).WithName("GetRangoId"); // Nomeia a rota como "GetRangoId"


// O endpoint retorna um Task de CreatedAtRoute com um objeto RangoDTO
rangosEndPoints.MapPost("", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext rangoDbContext,             // Injeta o contexto do banco de dados RangoDbContext
    IMapper mapper,                            // Injeta o mapeador AutoMapper
    [FromBody] RangoCriacaoDTO rangoCriacaoDTO // Obt�m o corpo da requisi��o como um objeto RangoCriacaoDTO
) =>
{
    // Mapeia o DTO de cria��o para uma nova entidade Rango
    var rangoEntity = mapper.Map<Rango>(rangoCriacaoDTO);

    // Adiciona a nova entidade ao contexto do banco de dados
    rangoDbContext.Add(rangoEntity);

    // Salva as altera��es no banco de dados
    await rangoDbContext.SaveChangesAsync();

    // Mapeia a entidade salva de volta para um DTO de retorno
    var rangoReturn = mapper.Map<RangoDTO>(rangoEntity);

    // Retorna um resultado CreatedAtRoute com o DTO de retorno,
    // especificando a rota "GetRangoId" e passando o id da entidade rec�m-criada
    return TypedResults.CreatedAtRoute(rangoReturn, "GetRangoId", new { rangoId = rangoReturn.Id });
});


// O endpoint aceita um par�metro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou Ok
rangosComIdEndPoints.MapPut("", async Task<Results<NotFound, Ok>> (
    RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
    IMapper mapper,                 // Injeta o mapeador AutoMapper
    [FromBody] RangoToUpdateDTO rangoToUpdateDTO, // Obt�m o corpo da requisi��o como um objeto RangoToUpdateDTO
    int rangoId                          // Obt�m o par�metro de rota 'id' como um inteiro
    ) =>
{
    // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    // Verifica se a entidade foi encontrada
    if (rangoEntity == null)
    {
        // Retorna NotFound se a entidade n�o existir
        return TypedResults.NotFound();
    }

    // Mapeia as mudan�as do DTO para a entidade existente
    mapper.Map(rangoToUpdateDTO, rangoEntity);

    // Salva as altera��es no banco de dados
    await rangoDbContext.SaveChangesAsync();

    // Retorna Ok se as mudan�as foram salvas com sucesso
    return TypedResults.Ok();
});


// O endpoint aceita um par�metro de rota 'id' do tipo int e retorna um Task de Results que pode ser NotFound ou NoContent
rangosComIdEndPoints.MapDelete("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
    int rangoId                          // Obt�m o par�metro de rota 'id' como um inteiro
) =>
{
    // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    // Verifica se a entidade foi encontrada
    if (rangoEntity == null)
    {
        // Retorna NotFound se a entidade n�o existir
        return TypedResults.NotFound();
    }

    // Remove a entidade do contexto do banco de dados
    rangoDbContext.Rangos.Remove(rangoEntity);

    // Salva as altera��es no banco de dados
    await rangoDbContext.SaveChangesAsync();

    // Retorna NoContent se a remo��o foi bem-sucedida
    return TypedResults.NoContent();
});

app.Run();
