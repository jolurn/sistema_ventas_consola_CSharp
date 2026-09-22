using ConsoleApp_17_09_2026.Interfaces;

namespace ConsoleApp_17_09_2026.Interfaces
{
    // OCP/DIP: fábrica para crear IMetodoPago. Permite añadir nuevos métodos sin modificar consumidores.
    public interface IMetodoPagoFactory
    {
        IMetodoPago CrearMetodoPago(string opcion);
    }
}
