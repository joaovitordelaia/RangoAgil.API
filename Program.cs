using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndPointHandlers;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RangoDbContext>(o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.RegisterRangosEndPoints();//ele � um extension method, como voc� t� chamando ele dentro do app, O par�metro que ele est� recebendo aqui � o pr�prio app

app.RegisterIngredientesEndpoints();

app.Run();
