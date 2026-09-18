using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Modelos
{
    public class Sede
    {
        public string Nombre { get; set; }
        public Dictionary<Producto, int> Stock { get; set; }

        public Sede(string nombre)
        {
            Nombre = nombre;
            Stock = new Dictionary<Producto, int>();
        }

        public void AgregarStock(Producto producto, int cantidad)
        {
            if (Stock.ContainsKey(producto))
                Stock[producto] += cantidad;
            else
                Stock[producto] = cantidad;
        }

        public bool HayStock(Producto producto, int cantidad)
        {
            return Stock.ContainsKey(producto) && Stock[producto] >= cantidad;
        }

        public void ReducirStock(Producto producto, int cantidad)
        {
            if (HayStock(producto, cantidad))
                Stock[producto] -= cantidad;
            else
                throw new Exception($"Stock insuficiente en la sede {Nombre}");
        }
    }
}
