using ConsoleApp_17_09_2026.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Modelos
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public RolUsuario Rol { get; set; }

        public Usuario(string nombre, string clave, RolUsuario rol)
        {
            Nombre = nombre;
            Clave = clave;
            Rol = rol;
        }
    }
}
