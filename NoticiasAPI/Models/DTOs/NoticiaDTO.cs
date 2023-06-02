using Newtonsoft.Json;

namespace NoticiasAPI.Models.DTOs
{
    public class NoticiaDTO
    {
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }= "";
        public string Categoria { get; set; } = "";

        //Estas propiedades solo se usara cuando se haga un post

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int IdAutor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int IdCategoria { get; set; }
    }
}
