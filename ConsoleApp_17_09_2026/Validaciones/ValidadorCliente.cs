using ConsoleApp_17_09_2026.Interfaces;
using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Validaciones
{
    public class ValidadorCliente : IValidable
    {
        public bool Validar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 4)
                throw new Exception("La clave debe tener al menos 4 caracteres.");
            return true;
        }
    }
}
