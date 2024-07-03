using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndPointHandlers;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RangoDbContext>(o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.RegisterRangosEndPoints();//ele é um extension method, como você tá chamando ele dentro do app, O parâmetro que ele está recebendo aqui é o próprio app

app.RegisterIngredientesEndpoints();

app.Run();
