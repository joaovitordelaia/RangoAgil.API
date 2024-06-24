using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgil.API.Entities;

public class Rango
{
    [Key]/*chave primaria da tabela*/
    public int Id { get; set; }

    [Required] /*não null*/
    [MaxLength(200)]
    public required string Nome { get; set; }
    public ICollection<Ingrediente> Ingredientes { get; set; } = [];

    [SetsRequiredMembers]
    public Rango(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public Rango()
    {
            
    }


}

