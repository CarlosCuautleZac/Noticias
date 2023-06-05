using System;
using System.Collections.Generic;

namespace NoticiasAPI.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Noticia> Noticia { get; set; } = new List<Noticia>();
}
