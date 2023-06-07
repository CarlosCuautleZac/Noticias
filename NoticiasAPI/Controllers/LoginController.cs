using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        string rootpath = "";

        public LoginController(Sistem21NoticiasContext context, IWebHostEnvironment environment)
        {
            repository = new(context);
            rootpath = environment.WebRootPath;
        }

        [HttpPost]
        public IActionResult Login(LoginDTO usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Username))
                {
                    return BadRequest("El nombre de usuario no debe ir vacío. Escriba el nombre de usuario o el correo electronico");
                }

                if (string.IsNullOrWhiteSpace(usuario.Password))
                {
                    return BadRequest("La contraseña no debe ir vacía");
                }

                var usuario_conectado = repository.Get().SingleOrDefault(x => (x.NombreUsuario == usuario.Username || x.Email == usuario.Username) && x.Contraseña == usuario.Password);

                if (usuario_conectado == null)
                {
                    return Unauthorized("Nombre de usuario o contraseña incorrectas.");
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        private void GuardarImagen(string imagen, int idautor)
        {
            var directorio = rootpath + "/profiles/" + idautor;

            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var bytesimg = Convert.FromBase64String(imagen);

            var rutadelaimagen = $"{directorio}/1.png";

            System.IO.File.WriteAllBytes(rutadelaimagen, bytesimg);
        }



        [HttpPost("registrar")]
        public IActionResult Register(RegisterDTO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                ModelState.AddModelError("", "El nombre de usuario no debe ir vacío");
            }

            if (string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                ModelState.AddModelError("", "La contraseña no debe ir vacía");
            }

            if (string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                ModelState.AddModelError("", "El nombre no debe ir vacío");
            }

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                ModelState.AddModelError("", "El correo electronico no debe ir vacío");
            }

            if (repository.Get().Any(x => x.NombreUsuario == usuario.NombreUsuario))
            {
                ModelState.AddModelError("", "El nombre de usuario ya ha sido registrado.");
            }

            if (repository.Get().Any(x => x.Email == usuario.Email))
            {
                ModelState.AddModelError("", "El correo electronico ya ha sido registrado.");
            }

            if (ModelState.IsValid)
            {
                Usuario u = new()
                {
                    NombreUsuario = usuario.NombreUsuario.ToUpper(),
                    Contraseña = usuario.Contraseña,
                    Nombre = usuario.Nombre.ToUpper(),
                    Email = usuario.Email.ToUpper()
                };

                repository.Insert(u);

                if (!string.IsNullOrWhiteSpace(usuario.Foto))
                    GuardarImagen(usuario.Foto, u.Id);

                return Ok();
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors)
                       .Where(y => y.ErrorMessage != "")
                       .Select(y => y.ErrorMessage)
                       .ToList();
                string errorString = string.Join(", ", errors);
                return BadRequest(errorString);
            }

        }
    }
}
