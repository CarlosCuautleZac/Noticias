﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Models
{
    public class RegisterDTO
    {
        public string NombreUsuario { get; set; } = "";
        public string Contraseña { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Foto { get; set; } = "";
    }
}
