using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndPointHandlers;
using RangoAgil.API.Extensions;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RangoDbContext>(o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

builder.Services.AddProblemDetails();

if (!app.Environment.IsProduction())
{

    app.UseExceptionHandler();
    //app.UseExceptionHandler(configureApplicationBuilder => // forma antiga e eficaz
    //{
    //    configureApplicationBuilder.Run(async context =>
    //    {
    //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //        context.Response.ContentType = "text/html";
    //        await context.Response.WriteAsync("An unexpected problem happened.");
    //    });
    //});
}

app.RegisterRangosEndPoints();//ele é um extension method, como você tá chamando ele dentro do app, O parâmetro que ele está recebendo aqui é o próprio app
app.RegisterIngredientesEndpoints();

app.Run();
