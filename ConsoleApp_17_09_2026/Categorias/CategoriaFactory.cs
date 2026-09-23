using ConsoleApp_17_09_2026.Modelos;
using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Categorias
{
    public class CategoriaFactory : ICategoriaFactory
    {
        // ============================================================
        // CAMPO
        // ============================================================
        // Diccionario: opción (string) → receta que crea una Categoria.
        // Reemplaza al switch de RegistrarProducto.
        private readonly Dictionary<string, Func<Categoria>> _creadores;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public CategoriaFactory()
        {
            _creadores = new Dictionary<string, Func<Categoria>>
            {
                { "1", () => new CategoriaTecnologia() },
                { "2", () => new CategoriaRopa() },
                { "3", () => new CategoriaAlimento() }
            };
        }

        // ============================================================
        // MÉTODO: CrearCategoria
        // ============================================================
        public Categoria CrearCategoria(string opcion)
        {
            if (_creadores.ContainsKey(opcion))
                return _creadores[opcion]();

            return null;
        }

        // ============================================================
        // MÉTODO: Registrar (OCP)
        // ============================================================
        // Ejemplo: categoriaFactory.Registrar("4", () => new CategoriaJuguete());
        public void Registrar(string opcion, Func<Categoria> creador)
        {
            _creadores[opcion] = creador;
        }
    }
}
