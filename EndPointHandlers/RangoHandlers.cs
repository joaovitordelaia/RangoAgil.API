using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using RangoAgil.API.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.Models;
using RangoAgil.API.Entities;

namespace RangoAgil.API.EndPointHandlers;

public static class RangoHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangoNomeAsync(
        RangoDbContext rangoDbContext,
        IMapper mapper,
        ILogger<RangoDTO> logger,
        [FromQuery(Name = "name")] string? nomeRango
    )
    {
        // Busca a lista de entidades "Rango" que contém o nome fornecido (case-insensitive).
        var rangoEntity = await rangoDbContext.Rangos
            .Where(x => nomeRango == null || x.Nome.ToLower().Contains(nomeRango.ToLower()))
            .ToListAsync();

        // Se a lista estiver vazia, retorna um resultado "NoContent".
        if (rangoEntity == null || rangoEntity.Count <= 0)
        {
            logger.LogInformation("Rango não encontrado.");
            return TypedResults.NoContent();
        }
        else
        {
            logger.LogInformation("Retornando o Rango encontrado.");
            // Mapeia a lista de entidades "Rango" para DTOs.
            var rangoEntiDto = mapper.Map<IEnumerable<RangoDTO>>(rangoEntity);

            // Retorna um resultado "Ok" com a lista de DTOs mapeados.
            return TypedResults.Ok(rangoEntiDto);
        }
    }

    public static async Task<Results<NoContent, Ok<RangoDTO>>> GetRangoIdAsync
        (RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
        IMapper mapper,                 // Injeta o mapeador AutoMapper
        int rangoId                     // Obtém o parâmetro de rota 'rangoId' como um inteiro
    )
    {
        // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
        var rango = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        // Verifica se a entidade foi encontrada
        if (rango == null)
        {
            // Retorna NoContent se a entidade não existir
            return TypedResults.NoContent();
        }

        // Mapeia a entidade para um DTO de retorno
        var retornoRango = mapper.Map<RangoDTO>(rango);

        // Retorna Ok com o DTO de retorno se a entidade foi encontrada
        return TypedResults.Ok(retornoRango);

    }

    public static async Task<CreatedAtRoute<RangoDTO>> PostRangoAsync
        (RangoDbContext rangoDbContext,             // Injeta o contexto do banco de dados RangoDbContext
        IMapper mapper,                            // Injeta o mapeador AutoMapper
        [FromBody] RangoCriacaoDTO rangoCriacaoDTO // Obtém o corpo da requisição como um objeto RangoCriacaoDTO
    )
    {
        // Mapeia o DTO de criação para uma nova entidade Rango
        var rangoEntity = mapper.Map<Rango>(rangoCriacaoDTO);

        // Adiciona a nova entidade ao contexto do banco de dados
        rangoDbContext.Add(rangoEntity);

        // Salva as alterações no banco de dados
        await rangoDbContext.SaveChangesAsync();

        // Mapeia a entidade salva de volta para um DTO de retorno
        var rangoReturn = mapper.Map<RangoDTO>(rangoEntity);

        // Retorna um resultado CreatedAtRoute com o DTO de retorno,
        // especificando a rota "GetRangoId" e passando o id da entidade recém-criada
        return TypedResults.CreatedAtRoute(rangoReturn, "GetRangoId", new
        {
            rangoId = rangoReturn.Id
        });
    }

    public static async Task<Results<NotFound, Ok>> PutRangoAsync
        (RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
        IMapper mapper,                 // Injeta o mapeador AutoMapper
        [FromBody] RangoToUpdateDTO rangoToUpdateDTO, // Obtém o corpo da requisição como um objeto RangoToUpdateDTO
        int rangoId                          // Obtém o parâmetro de rota 'id' como um inteiro
        )
    {
        // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        // Verifica se a entidade foi encontrada
        if (rangoEntity == null)
        {
            // Retorna NotFound se a entidade não existir
            return TypedResults.NotFound();
        }

        // Mapeia as mudanças do DTO para a entidade existente
        mapper.Map(rangoToUpdateDTO, rangoEntity);

        // Salva as alterações no banco de dados
        await rangoDbContext.SaveChangesAsync();

        // Retorna Ok se as mudanças foram salvas com sucesso
        return TypedResults.Ok();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync
        (RangoDbContext rangoDbContext,  // Injeta o contexto do banco de dados RangoDbContext
        int rangoId                          // Obtém o parâmetro de rota 'id' como um inteiro
    )
    {
        // Tenta encontrar a entidade Rango no banco de dados com o id fornecido
        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

        // Verifica se a entidade foi encontrada
        if (rangoEntity == null)
        {
            // Retorna NotFound se a entidade não existir
            return TypedResults.NotFound();
        }

        // Remove a entidade do contexto do banco de dados
        rangoDbContext.Rangos.Remove(rangoEntity);

        // Salva as alterações no banco de dados
        await rangoDbContext.SaveChangesAsync();

        // Retorna NoContent se a remoção foi bem-sucedida
        return TypedResults.NoContent();
    }




}
