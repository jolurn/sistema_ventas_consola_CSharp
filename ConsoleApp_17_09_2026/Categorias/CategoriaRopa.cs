using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Categorias
{
    public class CategoriaRopa : Categoria

    {
        public CategoriaRopa() : base("Ropa", 10) { }

        public override string Descripcion() => "Prendas de vestir";
    }
}
