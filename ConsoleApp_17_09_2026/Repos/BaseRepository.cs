using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp_17_09_2026.Repos
{
    // LSP/OCP: clase base que otras pueden heredar y extender sin romper contrato.
    public abstract class BaseRepository<T> : IRepository<T>
    {
        // protected para permitir que subclases accedan directamente al almacenamiento
        protected readonly List<T> _items;
        // readonly: demostración de variables inmutables en tiempo de ejecución
        protected readonly object _lock = new object();

        protected BaseRepository()
        {
            _items = new List<T>();
        }

        // Permite usar una lista existente como respaldo (evita duplicar datos)
        protected BaseRepository(List<T> initial)
        {
            _items = initial ?? new List<T>();
        }

        public virtual void Add(T item)
        {
            if (item == null) throw new RepositoryException("Item null al agregar");
            lock (_lock)
            {
                _items.Add(item);
            }
        }

        public virtual void Update(T item)
        {
            // Implementación por defecto: si no hay identidad definida, lanzar excepción
            throw new RepositoryException("Update no implementado para este repositorio.");
        }

        public virtual void Remove(T item)
        {
            if (item == null) throw new RepositoryException("Item null al eliminar");
            lock (_lock)
            {
                _items.Remove(item);
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            // retornamos una copia para evitar modificaciones externas directamente
            lock (_lock)
            {
                return _items.ToList();
            }
        }

        public virtual T GetById(Func<T, bool> predicate)
        {
            if (predicate == null) throw new RepositoryException("Predicate null");
            lock (_lock)
            {
                return _items.FirstOrDefault(predicate);
            }
        }

        public virtual IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null) throw new RepositoryException("Predicate null");
            lock (_lock)
            {
                return _items.Where(predicate).ToList();
            }
        }
    }
}
