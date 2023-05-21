using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoticiasAPI.Models;
using NoticiasAPI.Models.DTOs;
using NoticiasAPI.Repositories;
using System.Security.Claims;

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

                List<Claim> cliam = new()
                {

                };


                return Ok();
            }

        }
    }
}
