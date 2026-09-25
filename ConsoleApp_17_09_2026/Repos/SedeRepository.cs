using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp_17_09_2026.Repos
{
    public class SedeRepository : BaseRepository<Sede>
    {
        public SedeRepository() : base() { }
        public SedeRepository(System.Collections.Generic.List<Sede> backing) : base(backing) { }

        public override void Update(Sede item)
        {
            if (item == null) throw new RepositoryException("Sede nula");
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(s => s.Nombre == item.Nombre);
                if (existing == null) throw new RepositoryException("Sede no encontrada");
                // actualizar stock: reemplazar diccionario
                existing.Stock = item.Stock;
            }
        }

        public Sede GetByName(string nombre)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(s => s.Nombre == nombre);
            }
        }
    }
}
