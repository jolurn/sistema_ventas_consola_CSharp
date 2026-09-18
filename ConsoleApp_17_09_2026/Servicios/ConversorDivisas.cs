using ConsoleApp_17_09_2026.Enums;
using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Servicios
{
    public class ConversorDivisas : IConvertibleDivisa
    {
        // Tasas de cambio base respecto a Soles
        private readonly Dictionary<TipoMoneda, decimal> tasas = new Dictionary<TipoMoneda, decimal>()
        {
            { TipoMoneda.PEN, 1m },
            { TipoMoneda.USD, 0.27m },
            { TipoMoneda.EUR, 0.25m },
            { TipoMoneda.BTC, 0.0000027m },
            { TipoMoneda.ETH, 0.000045m }
        };

        public decimal Convertir(decimal monto, TipoMoneda origen, TipoMoneda destino)
        {
            decimal enSoles = monto / tasas[origen];
            return enSoles * tasas[destino];
        }

        public decimal CalcularComision(decimal monto, TipoMoneda moneda)
        {
            // 2% para cripto, 1% para el resto
            decimal porcentaje = (moneda == TipoMoneda.BTC || moneda == TipoMoneda.ETH) ? 0.02m : 0.01m;
            return monto * porcentaje;
        }
    }
}
