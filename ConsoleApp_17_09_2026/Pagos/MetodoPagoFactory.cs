using ConsoleApp_17_09_2026.Interfaces;
using System;

namespace ConsoleApp_17_09_2026.Pagos
{
    // MetodoPagoFactory aplica OCP y DIP: la creación de objetos de pago queda centralizada
    // y se expone vía la interfaz IMetodoPagoFactory. Para añadir un nuevo método de pago
    // solo hay que extender esta fábrica; no es necesario cambiar la lógica de venta.
    public class MetodoPagoFactory : IMetodoPagoFactory
    {
        public IMetodoPago CrearMetodoPago(string opcion)
        {
            switch (opcion)
            {
                case "1": return new PagoYape();
                case "2": return new PagoTarjeta();
                case "3": return new PagoCripto();
                default: return null;
            }
        }
    }
}
