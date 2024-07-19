using Microsoft.AspNetCore.Mvc;

namespace RangoAgil.API.EndPointFilters;

public  class RangoLocked : IEndpointFilter
{
    public int _rangoLocked { get; set; }

    public RangoLocked(int lockedRangoId)
    {
        _rangoLocked = lockedRangoId;
    }

    public async ValueTask<object?>  InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int rangoId;
        
        if (context.HttpContext.Request.Method == "PUT")
        {
            rangoId = context.GetArgument<int>(3);
        }
        else if (context.HttpContext.Request.Method == "DELETE")
        {
            rangoId = context.GetArgument<int>(1);
        }
        else
        {
            throw new NotSupportedException("Essa Receita não pode ser alterada ou deletada.");
        }

        if (rangoId == _rangoLocked)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Não pode modificar o tropeiro",
                Detail = "Essa Receita não pode ser alterada ou deletada."

            });
        }
        var result = await next.Invoke(context);
        return result;



    }
}

