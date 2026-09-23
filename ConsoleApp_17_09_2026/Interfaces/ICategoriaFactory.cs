using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Interfaces
{
    // OCP/DIP: fábrica para crear Categorías.
    // Permite añadir nuevas categorías sin modificar los consumidores.
    public interface ICategoriaFactory
    {
        // Crea una categoría a partir de una opción (ej. "1", "2", "3")
        Categoria CrearCategoria(string opcion);

        // Permite registrar nuevas categorías sin modificar la fábrica
        void Registrar(string opcion, Func<Categoria> creador);
    }
}
