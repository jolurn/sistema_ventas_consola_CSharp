using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Pagos
{
    public class PagoCripto : IMetodoPago
    {
        public string Nombre => "Cripto";

        public bool Pagar(decimal monto)
        {
            Console.WriteLine($"Pagando equivalente a S/ {monto} en cripto... [OK]");
            return true;
        }
    }
}
