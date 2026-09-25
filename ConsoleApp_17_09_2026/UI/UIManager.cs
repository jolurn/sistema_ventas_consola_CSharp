using ConsoleApp_17_09_2026.Modelos;
using ConsoleApp_17_09_2026.Repos;
using ConsoleApp_17_09_2026.Interfaces;
using ConsoleApp_17_09_2026.Categorias;
using System;
using System.Linq;

namespace ConsoleApp_17_09_2026.UI
{
    // SRP: UIManager es responsable solo de la interacción por consola (separado de Program/Main)
    public class UIManager
    {
        private readonly Interfaces.IRepository<Usuario> _usuarios;
        private readonly Interfaces.IRepository<Producto> _productos;
        private readonly Interfaces.IRepository<Sede> _sedes;
        private Usuario usuarioActual;

        // DIP: UIManager depende de abstracciones (IRepository<T>) y no de implementaciones concretas
        public UIManager(Interfaces.IRepository<Usuario> usuarios, Interfaces.IRepository<Producto> productos, Interfaces.IRepository<Sede> sedes)
        {
            _usuarios = usuarios;
            _productos = productos;
            _sedes = sedes;
        }

        public void Run()
        {
            // Replicar exactamente el flujo original: primer menú (Login/Salir), luego MenuPrincipal según rol
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA DE VENTAS ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Salir");
                Console.Write("Elige opción: ");

                var opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        try
                        {
                            Login();
                            if (usuarioActual != null)
                                MenuPrincipal();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] {ex.Message}");
                            Console.WriteLine("ENTER para continuar..."); Console.ReadLine();
                        }
                        break;
                    case "2": salir = true; break;
                    default:
                        Console.WriteLine("[ERROR] Opción inválida. ENTER..."); Console.ReadLine();
                        break;
                }
            }
        }

        // MenuPrincipal replicará el comportamiento original respetando roles
        private void MenuPrincipal()
        {
            bool volver = false;
            while (!volver)
            {
                Console.Clear();
                Console.WriteLine($"=== MENÚ ({usuarioActual.Rol}) ===");
                Console.WriteLine();

                Console.WriteLine("--- CONFIGURACIÓN ---");
                if (usuarioActual.Rol == Enums.RolUsuario.Admin)
                {
                    Console.WriteLine("1. Registrar sede");
                    Console.WriteLine("2. Registrar usuario");
                }

                if (usuarioActual.Rol == Enums.RolUsuario.Admin || usuarioActual.Rol == Enums.RolUsuario.Vendedor)
                {
                    Console.WriteLine("3. Registrar producto");
                    Console.WriteLine("4. Agregar stock a sede");
                }

                Console.WriteLine();
                Console.WriteLine("--- OPERACIÓN ---");
                Console.WriteLine("5. Ver productos");
                Console.WriteLine("6. Convertir divisas");
                Console.WriteLine("7. Registrar venta");

                Console.WriteLine();
                Console.WriteLine("--- SESIÓN ---");
                Console.WriteLine("0. Cerrar sesión");
                Console.WriteLine();

                Console.Write("Elige opción: ");
                string op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        if (usuarioActual.Rol == Enums.RolUsuario.Admin) Program.RegistrarSede();
                        else Program.AccesoDenegado();
                        break;
                    case "2":
                        if (usuarioActual.Rol == Enums.RolUsuario.Admin) Program.RegistrarUsuario();
                        else Program.AccesoDenegado();
                        break;
                    case "3":
                        if (usuarioActual.Rol == Enums.RolUsuario.Admin || usuarioActual.Rol == Enums.RolUsuario.Vendedor) Program.RegistrarProducto();
                        else Program.AccesoDenegado();
                        break;
                    case "4":
                        if (usuarioActual.Rol == Enums.RolUsuario.Admin || usuarioActual.Rol == Enums.RolUsuario.Vendedor) Program.AgregarStock();
                        else Program.AccesoDenegado();
                        break;
                    case "5": Program.VerProductos(); break;
                    case "6": Program.ConvertirDivisas(); break;
                    case "7": Program.RegistrarVenta(); break;
                    case "0":
                        usuarioActual = null;
                        volver = true;
                        break;
                    default:
                        Console.WriteLine("[ERROR] Opción inválida. ENTER...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        void Login()
        {
            Console.Clear();
            Console.WriteLine("--- LOGIN ---");
            Console.Write("Usuario: "); var nombre = Console.ReadLine();
            Console.Write("Clave: "); var clave = Console.ReadLine();
            var u = _usuarios.GetById(x => x.Nombre == nombre && x.Clave == clave);
            if (u == null)
            {
                Console.WriteLine("Credenciales inválidas. ENTER..."); Console.ReadLine();
                usuarioActual = null;
                return;
            }

            usuarioActual = u;
            Console.WriteLine($"Bienvenido {u.Nombre} ({u.Rol})"); Console.ReadLine();
        }

        void RegistrarUsuario()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR USUARIO ---");
            Console.Write("Nombre: "); var nombre = Console.ReadLine();
            Console.Write("Clave: "); var clave = Console.ReadLine();
            Console.WriteLine("Rol (0=Admin, 1=Vendedor, 2=Cliente): ");
            if (!int.TryParse(Console.ReadLine(), out int rolNum)) rolNum = 2;

            var user = new Usuario(nombre, clave, (Enums.RolUsuario)rolNum);
            _usuarios.Add(user);
            Console.WriteLine("Usuario registrado. ENTER..."); Console.ReadLine();
        }

        void RegistrarProducto()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR PRODUCTO ---");
            Console.Write("Nombre: "); var nombre = Console.ReadLine();
            Console.Write("Precio: "); decimal.TryParse(Console.ReadLine(), out decimal precio);

            Console.WriteLine("Categoría: 1=Tecnología 2=Ropa 3=Alimento");
            var opcion = Console.ReadLine();
            Categoria categoria = null;
            switch (opcion)
            {
                case "1": categoria = new CategoriaTecnologia(); break;
                case "2": categoria = new CategoriaRopa(); break;
                default: categoria = new CategoriaAlimento(); break;
            }

            var prod = new Producto(nombre, precio, categoria);
            _productos.Add(prod);
            Console.WriteLine("Producto registrado. ENTER..."); Console.ReadLine();
        }

        void AgregarStock()
        {
            Console.Clear();
            if (_sedes.GetAll().Count() == 0 || _productos.GetAll().Count() == 0)
            {
                Console.WriteLine("Primero registra al menos una sede y un producto."); Console.ReadLine(); return;
            }

            Console.WriteLine("Sedes:");
            var sedes = _sedes.GetAll().ToList();
            for (int i = 0; i < sedes.Count; i++) Console.WriteLine($"{i+1}. {sedes[i].Nombre}");
            Console.Write("Elige sede: "); int s = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Productos:");
            var prods = _productos.GetAll().ToList();
            for (int i = 0; i < prods.Count; i++) Console.WriteLine($"{i+1}. {prods[i].Nombre}");
            Console.Write("Elige producto: "); int p = int.Parse(Console.ReadLine()) - 1;
            Console.Write("Cantidad a agregar: "); int cant = int.Parse(Console.ReadLine());

            var sede = sedes[s];
            sede.AgregarStock(prods[p], cant);
            _sedes.Update(sede);

            Console.WriteLine("Stock actualizado. ENTER..."); Console.ReadLine();
        }

        void VerProductos()
        {
            // Simple wrapper que llama al método en Program para reutilizar lógica compleja ya existente
            Program.VerProductos();
        }
    }
}
