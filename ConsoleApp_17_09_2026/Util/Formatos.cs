using System;

namespace ConsoleApp_17_09_2026.Util
{
    public static class Formatos
    {
        public static string FormatoSoles(decimal monto)
        {
            return monto.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-PE"));
        }

        public static string FormatoMoneda(decimal monto, string culture = "es-PE")
        {
            return monto.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo(culture));
        }
    }
}
