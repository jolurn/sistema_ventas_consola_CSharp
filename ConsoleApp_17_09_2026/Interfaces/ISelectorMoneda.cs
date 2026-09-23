using ConsoleApp_17_09_2026.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Interfaces
{
    public interface ISelectorMoneda
    {
        // Devuelve la moneda correspondiente a una opción (ej. 1, 2, 3, 4)
        TipoMoneda Seleccionar(int opcion);

        // Permite registrar nuevas opciones sin modificar esta clase
        void Registrar(int opcion, TipoMoneda moneda);
    }
}
