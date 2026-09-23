using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;

namespace ConsoleApp_17_09_2026.Pagos
{
    public class MetodoPagoFactory : IMetodoPagoFactory
    {
        // ============================================================
        // CAMPO
        // ============================================================
        // Diccionario: clave (string) → función que crea un IMetodoPago.
        // Reemplaza al switch. Aquí guardamos "recetas" para crear cada pago.
        private readonly Dictionary<string, Func<IMetodoPago>> _creadores;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public MetodoPagoFactory()
        {
            _creadores = new Dictionary<string, Func<IMetodoPago>>
            {
                { "1", () => new PagoYape() },
                { "2", () => new PagoTarjeta() },
                { "3", () => new PagoCripto() }
            };
        }

        // ============================================================
        // MÉTODO: CrearMetodoPago
        // ============================================================
        // Recibe una opción y devuelve el método de pago. Si no existe, null.
        public IMetodoPago CrearMetodoPago(string opcion)
        {
            if (_creadores.ContainsKey(opcion))
                return _creadores[opcion]();

            return null;
        }

        // ============================================================
        // MÉTODO: Registrar (para OCP real)
        // ============================================================
        // Permite agregar nuevos métodos de pago SIN modificar esta clase.
        // Ejemplo: pagoFactory.Registrar("4", () => new PagoPlin());
        public void Registrar(string opcion, Func<IMetodoPago> creador)
        {
            _creadores[opcion] = creador;
        }
    }
}
