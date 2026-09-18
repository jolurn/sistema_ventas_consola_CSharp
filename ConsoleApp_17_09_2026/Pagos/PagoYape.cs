using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Pagos
{
    public class PagoYape : IMetodoPago
    {
        public string Nombre => "Yape";

        public bool Pagar(decimal monto)
        {
            Console.WriteLine($"Pagando S/ {monto} con Yape... [OK]");
            return true;
        }
    }
}
