using AutoMapper;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.Profiles
{
    public class RangoAgilProfile : Profile
    {
        public RangoAgilProfile()// reponsa de mapear o dominio
        {
            CreateMap<Rango, RangoDTO>().ReverseMap();
            CreateMap<Rango, RangoCriacaoDTO>().ReverseMap();
            CreateMap<Rango, RangoToUpdateDTO>().ReverseMap();
            /*estamos criando o vinculo da DTO com o dominio base dele,
             e o reversemap é basicamente dizer que é de dois caminhos
            rangoDTO é de Rango e Rango é de RangoDTO*/
            CreateMap<Ingrediente, IngredienteDTO>()
                .ForMember(d => d.RangoId,
                           o => o.MapFrom(s => s.Rangos.First().Id));
        }
    }
}
