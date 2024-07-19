using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"]));
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<RangoDbContext>();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromBrazil", policy =>
     policy
           .RequireRole("admin")
           .RequireClaim("country", "Brazil"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("TokenAuthRango",
        new()
        {
            Name = "Authorization",
            Description = "Token baseado em Autenticação e Autorização",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        }
    );
    options.AddSecurityRequirement(
        new()
        {
            {
                new()
                {
                            Reference = new OpenApiReference 
                    {
                            Type = ReferenceType.SecurityScheme,
                            Id = "TokenAuthRango"
                    }
                },
                new List<string>()
            }
        }
    );
});

var app = builder.Build();



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

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.RegisterRangosEndPoints();//ele é um extension method, como você tá chamando ele dentro do app, O parâmetro que ele está recebendo aqui é o próprio app
app.RegisterIngredientesEndpoints();

app.Run();
