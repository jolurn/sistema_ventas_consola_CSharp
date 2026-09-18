using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Modelos
{
    public class Producto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public Categoria Categoria { get; set; }

        public Producto(string nombre, decimal precio, Categoria categoria)
        {
            Nombre = nombre;
            Precio = precio;
            Categoria = categoria;
        }

        public decimal PrecioConImpuesto()
        {
            decimal conImpuesto = Precio + (Precio * Categoria.Impuesto / 100);
            return Math.Round(conImpuesto, 2, MidpointRounding.AwayFromZero);
        }
    }
}
