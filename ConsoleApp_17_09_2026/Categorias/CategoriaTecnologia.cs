using ConsoleApp_17_09_2026.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026.Categorias
{
    public class CategoriaTecnologia : Categoria
    {
        public CategoriaTecnologia() : base("Tecnología", 18) { }

        public override string Descripcion() => "Productos electrónicos y tecnológicos";
    }
}
