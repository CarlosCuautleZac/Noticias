using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Models;
using NoticiasAPI.Models.DTOs;
using NoticiasAPI.Repositories;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiasController : ControllerBase
    {
        Repository<Noticia> repositoryNoticias;

        public NoticiasController(Sistem21NoticiasContext context)
        {
            repositoryNoticias=new(context);
        }

        public IActionResult GetAll()
        {
            var noticias = repositoryNoticias.Get().Include(x=>x.IdUsuarioNavigation).Include(x=>x.IdCategoriaNavigation).Select(x=> new NoticiaDTO()
            {
                Titulo = x.Titulo,
                Autor = x.IdUsuarioNavigation.Nombre,
                Descripcion = x.Descripcion,
                Fecha=x.Fecha,
                Categoria = x.IdCategoriaNavigation.Nombre
            });

            return Ok(noticias);    
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult GetAllTest()
        {
            var noticias = repositoryNoticias.Get().Include(x => x.IdUsuarioNavigation).Include(x => x.IdCategoriaNavigation).Select(x => new NoticiaDTO()
            {
                Titulo = x.Titulo,
                Autor = x.IdUsuarioNavigation.Nombre,
                Descripcion = x.Descripcion,
                Fecha = x.Fecha,
                Categoria = x.IdCategoriaNavigation.Nombre
            });

            return Ok(noticias);
        }

    }
}
