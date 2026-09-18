using ConsoleApp_17_09_2026.Enums;
using ConsoleApp_17_09_2026.Interfaces;
using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Validaciones
{
    public class ValidadorAdmin : IValidable
    {
        public bool Validar(Usuario usuario)
        {
            if (usuario.Rol != RolUsuario.Admin)
                throw new UnauthorizedAccessException("Solo administradores pueden ingresar aquí.");
            return true;
        }
    }
}
