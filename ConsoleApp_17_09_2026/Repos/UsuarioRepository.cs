using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp_17_09_2026.Repos
{
    // Ejemplo LSP: UsuarioRepository puede usarse donde BaseRepository<Usuario> sea esperado
    public class UsuarioRepository : BaseRepository<Usuario>
    {
        public UsuarioRepository() : base() { }
        public UsuarioRepository(System.Collections.Generic.List<Usuario> backing) : base(backing) { }

        public override void Update(Usuario item)
        {
            if (item == null) throw new RepositoryException("Usuario nulo");
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(u => u.Nombre == item.Nombre);
                if (existing == null) throw new RepositoryException("Usuario no encontrado");
                existing.Clave = item.Clave;
                existing.Rol = item.Rol;
            }
        }

        // Método específico
        public Usuario GetByCredentials(string nombre, string clave)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(u => u.Nombre == nombre && u.Clave == clave);
            }
        }
    }
}
