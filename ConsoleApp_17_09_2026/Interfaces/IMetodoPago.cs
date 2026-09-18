using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Interfaces
{
    public interface IMetodoPago
    {
        string Nombre { get; }
        bool Pagar(decimal monto);
    }
}
