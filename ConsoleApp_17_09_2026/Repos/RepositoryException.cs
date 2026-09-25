using System;

namespace ConsoleApp_17_09_2026.Repos
{
    // Clase de excepción para centralizar errores en repositorios
    public class RepositoryException : Exception
    {
        public RepositoryException() { }
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }
    }
}
