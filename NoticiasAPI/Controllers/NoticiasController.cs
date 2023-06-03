using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Helpers;
using NoticiasAPI.Models;
using NoticiasAPI.Models.DTOs;
using NoticiasAPI.Repositories;
using System.Linq.Expressions;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiasController : ControllerBase
    {

        Repository<Noticia> repositoryNoticias;
        string rootpath = "";

        public NoticiasController(Sistem21NoticiasContext context, IWebHostEnvironment environment)
        {
            repositoryNoticias = new(context);
            rootpath = environment.WebRootPath;

        }

        public IActionResult GetAll()
        {
            var noticias = repositoryNoticias.Get().Include(x => x.IdUsuarioNavigation).Include(x => x.IdCategoriaNavigation).ToList();
            var noticias_a_enviar = noticias.Select(x => new NoticiaDTO()
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Autor = x.IdUsuarioNavigation.Nombre,
                Descripcion = x.Descripcion,
                Fecha = x.Fecha,
                Categoria = x.IdCategoriaNavigation.Nombre,
                Imagen = GetImage(x.Id)
            }).ToList();

            return Ok(noticias_a_enviar);
        }

        //Metodo para toma la imagen de la noticia
        string GetImage(int idnoticia)
        {
            string host = HttpContext.Request.Host.Value;
            var imgpath = $"{rootpath}/img/{idnoticia}/1.png";
            imgpath = imgpath.Replace("\\", "/");
            var path = "";
            if (System.IO.File.Exists(imgpath))
                path = $"https://{host}/img/{idnoticia}/1.png";
            else
                path = "https://img.freepik.com/free-vector/oops-404-error-with-broken-robot-concept-illustration_114360-1932.jpg?w=740&t=st=1684858014~exp=1684858614~hmac=0d423870b2d5bc2483c6898024e33e62e88943547e01a5f45abfc6004add664c";

            return path;
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

        [HttpPost]
        public IActionResult Post(NoticiaDTO noticia)
        {
            try
            {
                //validar
                if (noticia.IdAutor <= 0)
                {
                    ModelState.AddModelError("", "La noticia debe contener un autor");
                }

                if (noticia.IdCategoria <= 0)
                {
                    ModelState.AddModelError("", "La noticia debe contener una categoria");
                }

                if (string.IsNullOrWhiteSpace(noticia.Titulo))
                {
                    ModelState.AddModelError("", "El titulo no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(noticia.Descripcion))
                {
                    ModelState.AddModelError("", "La descripcion no debe ir vacia");
                }

                if (noticia.Fecha > DateTime.Now.ToMexicoTime())
                {
                    ModelState.AddModelError("", "La fecha de la noticia no puede ser mayor a la fecha actual.");
                }

                if (string.IsNullOrWhiteSpace(noticia.Imagen))
                {
                    ModelState.AddModelError("", "La noticia debe contener una imagen.");
                }

                if (ModelState.IsValid)
                {
                    Noticia n = new()
                    {
                        Id = 0,
                        Titulo = noticia.Titulo,
                        Descripcion = noticia.Descripcion,
                        IdCategoria = noticia.IdCategoria,
                        IdUsuario = noticia.IdAutor,
                        Fecha = noticia.Fecha,
                        UltimaModificacion = DateTime.Now.ToMexicoTime()
                    };

                    repositoryNoticias.Insert(n);

                    GuardarImagen(noticia.Imagen, n.Id);

                    return Ok();
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count() > 0).ToList();
                    return BadRequest(errors);
                }

            }
            catch (Exception ex)
            {
                var errordb = "";
                if (ex.InnerException != null)
                {
                    errordb = "---" + ex.InnerException.Message;
                }

                return BadRequest(ex + errordb);
            }
        }

        private void GuardarImagen(string imagen, int idnoticia)
        {
            var directorio = rootpath + "/img/" + idnoticia;

            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var bytesimg = Convert.FromBase64String(imagen);

            var rutadelaimagen = $"{directorio}/1.png";

            System.IO.File.WriteAllBytes(rutadelaimagen, bytesimg);
        }
    }
}
