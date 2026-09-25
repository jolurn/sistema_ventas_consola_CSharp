using System;
using System.Collections.Generic;

namespace ConsoleApp_17_09_2026.Interfaces
{
    // ISP/DIP: Interfaces segregadas para lectura y lectura/escritura.
    public interface IReadOnlyRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(Func<T, bool> predicate);
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        void Add(T item);
        void Update(T item);
        void Remove(T item);
    }
}
