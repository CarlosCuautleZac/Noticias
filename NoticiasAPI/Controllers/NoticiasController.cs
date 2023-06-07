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
    [Authorize]
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var noticias = repositoryNoticias.Get().Include(x => x.IdUsuarioNavigation).Include(x => x.IdCategoriaNavigation).ToList();
            var noticias_a_enviar = noticias.Select(x => new NoticiaDTO()
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Autor = ObtenerNombreAbreviado(x.IdUsuarioNavigation.Nombre),
                Descripcion = x.Descripcion,
                Fecha = x.Fecha,
                Categoria = x.IdCategoriaNavigation.Nombre,
                IdAutor = x.IdUsuario,
                IdCategoria = x.IdCategoria,
                Imagen = GetImage(x.Id),
                ImagenAutor = GetAutor(x.IdUsuario)
            }).ToList();

            return Ok(noticias_a_enviar);
        }


        string ObtenerNombreAbreviado(string nombreCompleto)
        {
            string[] nombres = nombreCompleto.Split(' ');

            if (nombres.Length == 0)
            {
                return string.Empty;
            }

            string primerNombre = nombres[0];
            string inicial = string.Empty;

            if (!string.IsNullOrEmpty(primerNombre))
            {
                inicial = primerNombre.Substring(0, 1).ToUpper();
            }

            return $"{primerNombre} {inicial}.";
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


        string GetAutor(int idautor)
        {
            string host = HttpContext.Request.Host.Value;
            var imgpath = $"{rootpath}/profiles/{idautor}/1.png";
            imgpath = imgpath.Replace("\\", "/");
            var path = "";
            if (System.IO.File.Exists(imgpath))
                path = $"https://{host}/profiles/{idautor}/1.png";
            else
                path = $"https://{host}/profiles/user.png";

            return path;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetAllByCategoria(int id)
        {
            var noticias = repositoryNoticias.Get().Include(x => x.IdUsuarioNavigation).Include(x => x.IdCategoriaNavigation)
                .Where(x => x.IdCategoria == id).ToList();
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

            if (noticias_a_enviar.Count == 0)
                return NoContent();
            else
                return Ok(noticias_a_enviar);
        }

       
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

                //if (noticia.Fecha > DateTime.Now.ToMexicoTime())
                //{
                //    ModelState.AddModelError("", "La fecha de la noticia no puede ser mayor a la fecha actual.");
                //}

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
                        Fecha = DateTime.Now.ToMexicoTime(),
                        UltimaModificacion = DateTime.Now.ToMexicoTime()
                    };

                    repositoryNoticias.Insert(n);

                    GuardarImagen(noticia.Imagen, n.Id);

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


        //Hacer el put y delete

        [HttpPut]
        public IActionResult Put(NoticiaDTO noticia)
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

                //if (noticia.Fecha > DateTime.Now.ToMexicoTime())
                //{
                //    ModelState.AddModelError("", "La fecha de la noticia no puede ser mayor a la fecha actual.");
                //}

                if (ModelState.IsValid)
                {
                    var n = repositoryNoticias.Get().FirstOrDefault(x => x.Id == noticia.Id);

                    if (n != null)
                    {
                        n.Titulo = noticia.Titulo;
                        n.Descripcion = noticia.Descripcion;
                        n.IdCategoria = noticia.IdCategoria;
                        n.Fecha = DateTime.Now.ToMexicoTime();
                        n.UltimaModificacion = DateTime.Now.ToMexicoTime();

                        repositoryNoticias.Update(n);

                        if (!string.IsNullOrWhiteSpace(noticia.Imagen))
                            GuardarImagen(noticia.Imagen, n.Id);

                        return Ok();
                    }
                    else
                        return NotFound("La noticia no existe o ya se ha borrado");
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

        //idnoticia/idautor
        [HttpDelete("{idNoticia}/{idAutor}")]
        public IActionResult Delete(int idNoticia, int idAutor)
        {
            var noticia = repositoryNoticias.Get().FirstOrDefault(x => x.Id == idNoticia);

            if (noticia != null)
            {
                if (noticia.IdUsuario != idAutor)
                    return Unauthorized("No tiene permiso para eliminar una noticia que no sea suya.");
                else
                {
                    repositoryNoticias.Delete(noticia);
                    DeleteEvidence(noticia.Id);
                    return Ok();
                }
            }
            else
            {
                return NotFound("La noticia no existe o ya ha sido eliminada");
            }
        }

        private void DeleteEvidence(int id)
        {
            string path = $"{rootpath}/img/{id}/1.png";

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
