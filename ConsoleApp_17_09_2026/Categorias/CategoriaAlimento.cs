using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Categorias
{
    public class CategoriaAlimento : Categoria
    {
        public CategoriaAlimento() : base("Alimento", 5) { }

        public override string Descripcion() => "Productos comestibles";
    }
}
