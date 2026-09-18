using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Modelos
{
    public abstract class Categoria
    {
        public string Nombre { get; set; }
        public decimal Impuesto { get; set; } // % de impuesto por categoría

        protected Categoria(string nombre, decimal impuesto)
        {
            Nombre = nombre;
            Impuesto = impuesto;
        }

        public abstract string Descripcion();
    }
}
