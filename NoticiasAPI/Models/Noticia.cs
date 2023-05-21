using System;
using System.Collections.Generic;

namespace NoticiasAPI.Models;

public partial class Noticia
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public int IdUsuario { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime UltimaModificacion { get; set; }

    public string Descripcion { get; set; } = null!;

    public int IdCategoria { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
