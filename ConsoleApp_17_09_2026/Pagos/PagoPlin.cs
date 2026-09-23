using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Pagos
{
    internal class PagoPlin : IMetodoPago
    {
        public string Nombre => "Plin";

        public bool Pagar(decimal monto)
        {
            Console.WriteLine($"Pagando S/ {monto} con Plin... [OK]");
            return true;
        }
    }
}
