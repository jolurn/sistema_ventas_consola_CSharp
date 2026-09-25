using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp_17_09_2026.Repos
{
    public class ProductoRepository : BaseRepository<Producto>
    {
        public ProductoRepository() : base() { }
        public ProductoRepository(System.Collections.Generic.List<Producto> backing) : base(backing) { }

        public override void Update(Producto item)
        {
            if (item == null) throw new RepositoryException("Producto nulo");
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(p => p.Nombre == item.Nombre);
                if (existing == null) throw new RepositoryException("Producto no encontrado");
                existing.Precio = item.Precio;
                existing.Categoria = item.Categoria;
            }
        }

        public IEnumerable<Producto> ObtenerPorCategoria(string nombreCategoria)
        {
            lock (_lock)
            {
                return _items.Where(p => p.Categoria != null && p.Categoria.Nombre == nombreCategoria).ToList();
            }
        }
    }
}
