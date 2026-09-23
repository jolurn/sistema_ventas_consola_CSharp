using ConsoleApp_17_09_2026.Enums;
using ConsoleApp_17_09_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Servicios
{
    public class SelectorMoneda : ISelectorMoneda
    {
        // ============================================================
        // CAMPO
        // ============================================================
        // Diccionario: opción del menú (int) → TipoMoneda
        // Reemplaza al switch de ConvertirDivisas.
        private readonly Dictionary<int, TipoMoneda> _monedas;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================
        public SelectorMoneda()
        {
            _monedas = new Dictionary<int, TipoMoneda>
            {
                { 1, TipoMoneda.PEN },   // 👈 nuevo
                { 2, TipoMoneda.USD },
                { 3, TipoMoneda.EUR },
                { 4, TipoMoneda.BTC },
                { 5, TipoMoneda.ETH }
            };
        }

        // ============================================================
        // MÉTODO: Seleccionar
        // ============================================================
        // Si la opción no existe, devuelve PEN por defecto (o podrías lanzar excepción).
        public TipoMoneda Seleccionar(int opcion)
        {
            if (_monedas.ContainsKey(opcion))
                return _monedas[opcion];

            return TipoMoneda.PEN;  // opción inválida → PEN por defecto
        }

        // ============================================================
        // MÉTODO: Registrar (OCP)
        // ============================================================
        // Ejemplo: selector.Registrar(5, TipoMoneda.PEN);
        public void Registrar(int opcion, TipoMoneda moneda)
        {
            _monedas[opcion] = moneda;
        }
    }
}
