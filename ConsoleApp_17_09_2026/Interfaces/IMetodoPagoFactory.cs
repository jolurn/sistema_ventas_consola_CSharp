using ConsoleApp_17_09_2026.Interfaces;
using System;

namespace ConsoleApp_17_09_2026.Interfaces
{
    // OCP/DIP: fábrica para crear IMetodoPago. Permite añadir nuevos métodos sin modificar consumidores.
    public interface IMetodoPagoFactory
    {
        // Crea un método de pago a partir de una opción (ej. "1", "2", "3")
        IMetodoPago CrearMetodoPago(string opcion);

        // 👇 NUEVO: permite registrar nuevas opciones sin modificar la fábrica
        void Registrar(string opcion, Func<IMetodoPago> creador);
    }
}
