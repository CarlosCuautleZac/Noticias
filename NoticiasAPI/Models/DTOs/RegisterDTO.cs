namespace NoticiasAPI.Models.DTOs
{
    public class RegisterDTO
    {
        public string NombreUsuario { get; set; } = "";
        public string Contraseña { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = ""; 
    }
}
