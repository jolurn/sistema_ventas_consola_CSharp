using ConsoleApp_17_09_2026.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Interfaces
{
    public interface IConvertibleDivisa
    {
        decimal Convertir(decimal monto, TipoMoneda origen, TipoMoneda destino);
        decimal CalcularComision(decimal monto, TipoMoneda moneda);
    }
}
