using Newtonsoft.Json;

namespace NoticiasAPI.Models.DTOs
{
    public class NoticiaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }= "";
        public string Categoria { get; set; } = "";

        //Cuando sea para get se mandara un url de la imagen de la noticia que esta en la api, cuando sea post se enviara un base64
        public string Imagen { get; set; } = "";

        //Estas propiedades solo se usara cuando se haga un post

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int IdAutor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int IdCategoria { get; set; }
    }
}
