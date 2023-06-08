using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoticiasAPI.Models;
using NoticiasAPI.Models.DTOs;
using NoticiasAPI.Repositories;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        Repository<Categoria> repositoryCategoria;
        public CategoriaController(Sistem21NoticiasContext context)
        {
            repositoryCategoria = new(context);
        }

        public IActionResult Get()
        {
            var categorias= repositoryCategoria.Get().Select(x=> new CategoriaDTO() { Id = x.Id, Nombre =x.Nombre});
            return Ok(categorias);
        }
    }
}
