using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NoticiasAPI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

string? cadena = builder.Configuration.GetConnectionString("NoticiasConnectionStrings");
builder.Services.AddDbContext<Sistem21NoticiasContext>(optionsBuilder =>
optionsBuilder.UseMySql(cadena, ServerVersion.AutoDetect(cadena)),ServiceLifetime.Transient);


var app = builder.Build();

app.MapControllers();
app.Run();
