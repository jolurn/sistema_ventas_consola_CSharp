using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Pagos
{
    public class PagoTarjeta : IMetodoPago
    {
        public string Nombre => "Tarjeta";

        public bool Pagar(decimal monto)
        {
            Console.WriteLine($"Pagando S/ {monto} con Tarjeta... [OK]");
            return true;
        }
    }
}
