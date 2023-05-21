namespace NoticiasAPI.Models.DTOs
{
    public class NoticiaDTO
    {
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }= "";
        public string Categoria { get; set; } = "";
    }
}
