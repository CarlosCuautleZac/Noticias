using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoticiasAPI.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

string? cadena = builder.Configuration.GetConnectionString("NoticiasConnectionStrings");
builder.Services.AddDbContext<Sistem21NoticiasContext>(optionsBuilder =>
optionsBuilder.UseMySql(cadena, ServerVersion.AutoDetect(cadena)),ServiceLifetime.Transient);


//Datos de JWT
//ISSUER, AUDENCE, SECRET

string issuer = "noticias.sistemas19.com";
string audence = "mauinews";
var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuMiChiquitita83_"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwt => 
{
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer= issuer,
        ValidAudience=audence,
        IssuerSigningKey=secret,
        ValidateAudience=true,
        ValidateIssuer=true
    };

});


var app = builder.Build();

app.MapControllers();
app.UseAuthorization();
app.Run();
