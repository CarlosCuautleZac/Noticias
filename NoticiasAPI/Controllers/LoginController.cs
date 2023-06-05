using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoticiasAPI.Models;
using NoticiasAPI.Models.DTOs;
using NoticiasAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        Repository<Usuario> repository;

        public LoginController(Sistem21NoticiasContext context)
        {
            repository = new(context);
        }

        [HttpPost]
        public IActionResult Login(LoginDTO usuario)
        {
            var usuario_conectado = repository.Get().SingleOrDefault(x => x.NombreUsuario == usuario.Username && x.Contraseña == usuario.Password);

            if(usuario_conectado == null)
            {
                return Unauthorized("Nombre de usuario ó contraseña incorrecta");
            }
            else
            {
                //hacer lo de jwt
                //1. Crear Claims
                //2. Crear Token
                //3. Regresar el token

                List<Claim> cliams = new()
                {
                    new Claim("Id",usuario_conectado.Id.ToString()),
                    new Claim("Usuario", usuario_conectado.NombreUsuario),
                    new Claim(ClaimTypes.Name, usuario_conectado.Nombre),
                    new Claim(ClaimTypes.Email, usuario_conectado.Email)
                };

                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Issuer = "noticias.sistemas19.com",
                    Audience = "mauinews",
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TuMiChiquitita83_"))
                        , SecurityAlgorithms.HmacSha256),
                    Subject = new ClaimsIdentity(cliams, JwtBearerDefaults.AuthenticationScheme)
                };

                JwtSecurityTokenHandler handler = new();
                var token = handler.CreateToken(tokenDescriptor);

                return Ok(handler.WriteToken(token));
            }

        }
    }
}
