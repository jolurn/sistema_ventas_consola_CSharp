using ConsoleApp_17_09_2026.Categorias;
using ConsoleApp_17_09_2026.Enums;
using ConsoleApp_17_09_2026.Interfaces;
using ConsoleApp_17_09_2026.Modelos;
using ConsoleApp_17_09_2026.Pagos;
using ConsoleApp_17_09_2026.Servicios;
using ConsoleApp_17_09_2026.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_17_09_2026
{
    public class Program
    {
        // "Variables globales" para usar en todo el programa
        // SRP: Program sigue siendo la entrada, pero delega responsabilidades a servicios/fábricas
        static List<Usuario> usuarios = new List<Usuario>();
        static List<Producto> productos = new List<Producto>();
        static List<Sede> sedes = new List<Sede>();
        static Usuario usuarioActual = null;

        // DIP/OCP: Dependemos de abstracciones para crear métodos de pago y convertir divisas
        static Interfaces.IMetodoPagoFactory pagoFactory = new Pagos.MetodoPagoFactory(); // OCP/DIP
        static Interfaces.IConvertibleDivisa conversor = new Servicios.ConversorDivisas(); // DIP

        // OCP/DIP: fábrica de categorías
        static Interfaces.ICategoriaFactory categoriaFactory = new Categorias.CategoriaFactory();

        // OCP/DIP: selector de moneda para conversión de divisas
        static Interfaces.ISelectorMoneda selectorMoneda = new Servicios.SelectorMoneda();

        static void Main(string[] args)
        {
            // Datos precargados para no empezar vacío
            usuarios.Add(new Usuario("jorge", "123", RolUsuario.Admin));

            // OCP en acción: registrar PEN para conversiones
            selectorMoneda.Registrar(1, TipoMoneda.PEN);

            // 👇 OCP EN ACCIÓN: agregar Plin SIN modificar MetodoPagoFactory
            pagoFactory.Registrar("4", () => new PagoPlin());

            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA DE VENTAS ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Salir");
                Console.Write("Elige opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": Login(); break;
                    case "2": salir = true; break;
                    default:
                        Console.WriteLine("[ERROR] Opción inválida. ENTER...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // ============ LOGIN ============
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("--- LOGIN ---");
            Console.Write("Usuario: ");
            string nombre = Console.ReadLine();
            Console.Write("Clave: ");
            string clave = Console.ReadLine();

            usuarioActual = usuarios.Find(u => u.Nombre == nombre && u.Clave == clave);

            if (usuarioActual == null)
            {
                Console.WriteLine("[ERROR] Usuario o clave incorrectos. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"[OK] Bienvenido {usuarioActual.Nombre} ({usuarioActual.Rol})");
            Console.ReadLine();
            MenuPrincipal();
        }

        // ============ REGISTRAR USUARIO ============
        static void RegistrarUsuario()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR USUARIO ---");
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Clave: ");
            string clave = Console.ReadLine();

            Console.WriteLine("Rol (0=Admin, 1=Vendedor, 2=Cliente): ");
            int rolNum = int.Parse(Console.ReadLine());
            RolUsuario rol = (RolUsuario)rolNum;

            usuarios.Add(new Usuario(nombre, clave, rol));
            Console.WriteLine("[OK] Usuario registrado. ENTER para continuar...");
            Console.ReadLine();
        }

        // ============ MENÚ PRINCIPAL ============
        static void MenuPrincipal()
        {
            bool volver = false;
            while (!volver)
            {
                Console.Clear();
                Console.WriteLine($"=== MENÚ ({usuarioActual.Rol}) ===");
                Console.WriteLine();

                // --- CONFIGURACIÓN ---
                Console.WriteLine("--- CONFIGURACIÓN ---");

                if (usuarioActual.Rol == RolUsuario.Admin)
                {
                    Console.WriteLine("1. Registrar sede");
                    Console.WriteLine("2. Registrar usuario");
                }

                if (usuarioActual.Rol == RolUsuario.Admin || usuarioActual.Rol == RolUsuario.Vendedor)
                {
                    Console.WriteLine("3. Registrar producto");
                    Console.WriteLine("4. Agregar stock a sede");
                }

                // --- OPERACIÓN ---
                Console.WriteLine();
                Console.WriteLine("--- OPERACIÓN ---");
                Console.WriteLine("5. Ver productos");
                Console.WriteLine("6. Convertir divisas");
                Console.WriteLine("7. Registrar venta");

                // --- SESIÓN ---
                Console.WriteLine();
                Console.WriteLine("--- SESIÓN ---");
                Console.WriteLine("0. Cerrar sesión");
                Console.WriteLine();

                Console.Write("Elige opción: ");
                string op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        if (usuarioActual.Rol == RolUsuario.Admin)
                            RegistrarSede();
                        else AccesoDenegado();
                        break;
                    case "2":
                        if (usuarioActual.Rol == RolUsuario.Admin)
                            RegistrarUsuario();
                        else AccesoDenegado();
                        break;
                    case "3":
                        if (usuarioActual.Rol == RolUsuario.Admin || usuarioActual.Rol == RolUsuario.Vendedor)
                            RegistrarProducto();
                        else AccesoDenegado();
                        break;
                    case "4":
                        if (usuarioActual.Rol == RolUsuario.Admin || usuarioActual.Rol == RolUsuario.Vendedor)
                            AgregarStock();
                        else AccesoDenegado();
                        break;
                    case "5": VerProductos(); break;
                    case "6": ConvertirDivisas(); break;
                    case "7": RegistrarVenta(); break;
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

        // ============ REGISTRAR PRODUCTOS ============
        static void RegistrarProducto()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR PRODUCTO ---");
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Precio: ");
            decimal precio = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Categoría:");
            Console.WriteLine("1. Tecnología");
            Console.WriteLine("2. Ropa");
            Console.WriteLine("3. Alimento");
            Console.Write("Elige: ");
            string cat = Console.ReadLine();

            // 👇 OCP/DIP: usamos la fábrica en vez del switch
            Categoria categoria = categoriaFactory.CrearCategoria(cat);

            if (categoria == null)
            {
                Console.WriteLine("[ERROR] Categoría inválida. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            productos.Add(new Producto(nombre, precio, categoria));
            Console.WriteLine("[OK] Producto registrado. ENTER...");
            Console.ReadLine();
        }

        // ============ MÉTODO AUXILIAR: CALCULAR STOCK TOTAL ============
        static int StockTotalProducto(Producto producto)
        {
            int totalStock = 0;

            foreach (Sede sede in sedes)
            {
                if (sede.Stock.ContainsKey(producto))
                {
                    totalStock += sede.Stock[producto];
                }
            }

            return totalStock;
        }
        static void VerProductos()
        {
            Console.Clear();
            Console.WriteLine("--- PRODUCTOS ---");

            if (productos.Count == 0)
            {
                Console.WriteLine("No hay productos registrados.");
            }
            else
            {
                for (int i = 0; i < productos.Count; i++)
                {
                    var p = productos[i];

                    // Línea principal del producto
                    Console.WriteLine($"{i + 1}. {p.Nombre} | Precio: {FormatoSoles(p.Precio)} | " +
                                      $"Categoría: {p.Categoria.Nombre} | Con impuesto: {FormatoSoles(p.PrecioConImpuesto())} | " +
                                      $"Stock total: {StockTotalProducto(p)}");

                    // Desglose por sede
                    if (sedes.Count > 0)
                    {
                        foreach (Sede sede in sedes)
                        {
                            int stockSede = 0;
                            if (sede.Stock.ContainsKey(p))
                                stockSede = sede.Stock[p];

                            Console.WriteLine($"   -> {sede.Nombre}: {stockSede}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   -> (No hay sedes registradas)");
                    }

                    Console.WriteLine(); // línea en blanco entre productos
                }
            }

            Console.WriteLine("ENTER para volver...");
            Console.ReadLine();
        }

        // ============ SEDES ============
        static void RegistrarSede()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR SEDE ---");
            Console.Write("Nombre de la sede: ");
            string nombre = Console.ReadLine();
            sedes.Add(new Sede(nombre));
            Console.WriteLine("[OK] Sede registrada. ENTER...");
            Console.ReadLine();
        }

        static void AgregarStock()
        {
            Console.Clear();
            if (sedes.Count == 0 || productos.Count == 0)
            {
                Console.WriteLine("Primero registra al menos una sede y un producto.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("--- AGREGAR STOCK ---");
            Console.WriteLine("Sedes:");
            for (int i = 0; i < sedes.Count; i++)
                Console.WriteLine($"{i + 1}. {sedes[i].Nombre}");
            Console.Write("Elige sede: ");
            int s = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Productos:");
            for (int i = 0; i < productos.Count; i++)
                Console.WriteLine($"{i + 1}. {productos[i].Nombre}");
            Console.Write("Elige producto: ");
            int p = int.Parse(Console.ReadLine()) - 1;

            Console.Write("Cantidad a agregar: ");
            int cant = int.Parse(Console.ReadLine());

            sedes[s].AgregarStock(productos[p], cant);
            Console.WriteLine("[OK] Stock actualizado. ENTER...");
            Console.ReadLine();
        }

        // ============ DIVISAS ============
        static void ConvertirDivisas()
        {
            Console.Clear();
            Console.WriteLine("--- CONVERSIÓN DE DIVISAS ---");

            // ------------------------------------------------------------
            // 1. ELEGIR MONEDA DE ORIGEN
            // ------------------------------------------------------------
            Console.WriteLine("\n--- MONEDA DE ORIGEN ---");
            Console.WriteLine("1. PEN (Soles)");
            Console.WriteLine("2. USD (Dólares)");
            Console.WriteLine("3. EUR (Euros)");
            Console.WriteLine("4. BTC (Bitcoin)");
            Console.WriteLine("5. ETH (Ethereum)");
            Console.Write("Elige origen: ");

            int opOrigen;
            if (!int.TryParse(Console.ReadLine(), out opOrigen) || opOrigen < 1 || opOrigen > 5)
            {
                Console.WriteLine("[ERROR] Opción inválida. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            TipoMoneda origen = selectorMoneda.Seleccionar(opOrigen);

            // ------------------------------------------------------------
            // 2. INGRESAR MONTO
            // ------------------------------------------------------------
            Console.Write($"\nMonto en {origen}: ");
            decimal monto;
            if (!decimal.TryParse(Console.ReadLine(), out monto) || monto <= 0)
            {
                Console.WriteLine("[ERROR] Monto inválido. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 3. ELEGIR MONEDA DE DESTINO
            // ------------------------------------------------------------
            Console.WriteLine("\n--- MONEDA DE DESTINO ---");
            Console.WriteLine("1. PEN (Soles)");
            Console.WriteLine("2. USD (Dólares)");
            Console.WriteLine("3. EUR (Euros)");
            Console.WriteLine("4. BTC (Bitcoin)");
            Console.WriteLine("5. ETH (Ethereum)");
            Console.Write("Elige destino: ");

            int opDestino;
            if (!int.TryParse(Console.ReadLine(), out opDestino) || opDestino < 1 || opDestino > 5)
            {
                Console.WriteLine("[ERROR] Opción inválida. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            TipoMoneda destino = selectorMoneda.Seleccionar(opDestino);

            // Validar que origen y destino sean distintos
            if (origen == destino)
            {
                Console.WriteLine("[AVISO] Origen y destino son la misma moneda. No hay nada que convertir.");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 4. CONVERTIR Y MOSTRAR RESULTADO
            // ------------------------------------------------------------
            IConvertibleDivisa conv = conversor;
            decimal resultado = conv.Convertir(monto, origen, destino);
            decimal comision = conv.CalcularComision(resultado, destino);

            Console.WriteLine();
            Console.WriteLine("========== RESULTADO ==========");
            Console.WriteLine($"{FormatearMoneda(monto, origen)} = {FormatearMoneda(resultado, destino)}");
            Console.WriteLine($"Comisión: {FormatearMoneda(comision, destino)}");
            Console.WriteLine("===============================");
            Console.ReadLine();
        }

        // Método auxiliar: muestra mensaje de acceso denegado
        static void AccesoDenegado()
        {
            Console.WriteLine("[ERROR] No tienes permisos para esta opción.");
            Console.WriteLine("Presiona ENTER para volver...");
            Console.ReadLine();
        }

        // ============ REGISTRAR VENTA ============
        static void RegistrarVenta()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR VENTA ---");

            // Validaciones previas
            if (sedes.Count == 0)
            {
                Console.WriteLine("[AVISO] No hay sedes registradas.");
                Console.WriteLine("Presiona ENTER para volver...");
                Console.ReadLine();
                return;
            }

            if (productos.Count == 0)
            {
                Console.WriteLine("[AVISO] No hay productos registrados.");
                Console.WriteLine("Presiona ENTER para volver...");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 1. ELEGIR SEDE  👈 NUEVO
            // ------------------------------------------------------------
            Console.WriteLine("\n--- SELECCIONA LA SEDE ---");
            for (int i = 0; i < sedes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {sedes[i].Nombre}");
            }

            Console.Write("\nElige sede: ");
            int numeroSede;
            if (!int.TryParse(Console.ReadLine(), out numeroSede) ||
                numeroSede < 1 || numeroSede > sedes.Count)
            {
                Console.WriteLine("[ERROR] Sede inválida. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            Sede sedeElegida = sedes[numeroSede - 1];

            // ------------------------------------------------------------
            // 2. MOSTRAR PRODUCTOS CON STOCK DE ESA SEDE  👈 MODIFICADO
            // ------------------------------------------------------------
            Console.WriteLine($"\n--- PRODUCTOS EN {sedeElegida.Nombre.ToUpper()} ---");
            for (int i = 0; i < productos.Count; i++)
            {
                int stockEnSede = 0;
                if (sedeElegida.Stock.ContainsKey(productos[i]))
                    stockEnSede = sedeElegida.Stock[productos[i]];

                Console.WriteLine($"{i + 1}. {productos[i].Nombre} - {FormatoSoles(productos[i].PrecioConImpuesto())} | Stock: {stockEnSede}");
            }

            Console.Write("\nElige producto: ");
            int numeroProducto;
            if (!int.TryParse(Console.ReadLine(), out numeroProducto) ||
                numeroProducto < 1 || numeroProducto > productos.Count)
            {
                Console.WriteLine("[ERROR] Producto inválido. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            Producto productoElegido = productos[numeroProducto - 1];

            // ------------------------------------------------------------
            // 3. VERIFICAR STOCK EN LA SEDE
            // ------------------------------------------------------------
            int stockDisponible = 0;
            if (sedeElegida.Stock.ContainsKey(productoElegido))
                stockDisponible = sedeElegida.Stock[productoElegido];

            if (stockDisponible == 0)
            {
                Console.WriteLine($"[ERROR] No hay stock de '{productoElegido.Nombre}' en la sede {sedeElegida.Nombre}.");
                Console.WriteLine("Presiona ENTER para volver...");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 4. ELEGIR CANTIDAD
            // ------------------------------------------------------------
            Console.Write($"Cantidad (stock en {sedeElegida.Nombre}: {stockDisponible}): ");
            int cantidad;
            if (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0)
            {
                Console.WriteLine("[ERROR] Cantidad inválida. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            if (cantidad > stockDisponible)
            {
                Console.WriteLine($"[ERROR] Stock insuficiente. Solo hay {stockDisponible} unidades en {sedeElegida.Nombre}.");
                Console.WriteLine("Presiona ENTER para volver...");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 5. DETERMINAR CLIENTE
            // ------------------------------------------------------------
            Usuario clienteVenta;

            if (usuarioActual.Rol == RolUsuario.Cliente)
            {
                clienteVenta = usuarioActual;
                Console.WriteLine($"\nCliente: {clienteVenta.Nombre} (tú mismo)");
            }
            else
            {
                List<Usuario> clientes = usuarios.FindAll(u => u.Rol == RolUsuario.Cliente);

                if (clientes.Count == 0)
                {
                    Console.WriteLine("\n[AVISO] No hay clientes registrados. Se usará 'Consumidor Final'.");
                    clienteVenta = new Usuario("Consumidor Final", "-", RolUsuario.Cliente);
                }
                else
                {
                    Console.WriteLine("\n--- CLIENTES ---");
                    for (int i = 0; i < clientes.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {clientes[i].Nombre}");
                    }

                    Console.Write("Elige cliente: ");
                    int numCliente;
                    if (!int.TryParse(Console.ReadLine(), out numCliente) ||
                        numCliente < 1 || numCliente > clientes.Count)
                    {
                        Console.WriteLine("[ERROR] Cliente inválido. ENTER para volver...");
                        Console.ReadLine();
                        return;
                    }

                    clienteVenta = clientes[numCliente - 1];
                }
            }

            // ------------------------------------------------------------
            // 6. CALCULAR TOTAL
            // ------------------------------------------------------------
            decimal precioUnitario = productoElegido.PrecioConImpuesto();
            decimal total = precioUnitario * cantidad;

            Console.WriteLine();
            Console.WriteLine("========== RESUMEN ==========");
            Console.WriteLine($"Sede: {sedeElegida.Nombre}");
            Console.WriteLine($"Cliente: {clienteVenta.Nombre}");
            Console.WriteLine($"Vendedor: {usuarioActual.Nombre}");
            Console.WriteLine($"Producto: {productoElegido.Nombre}");
            Console.WriteLine($"Categoría: {productoElegido.Categoria.Nombre}");
            Console.WriteLine($"Precio unitario: {FormatoSoles(precioUnitario)}");
            Console.WriteLine($"Cantidad: {cantidad}");
            Console.WriteLine($"Total a pagar: {FormatoSoles(total)}");
            Console.WriteLine("=============================");

            // ------------------------------------------------------------
            // 7. ELEGIR MÉTODO DE PAGO
            // ------------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("--- MÉTODO DE PAGO ---");
            Console.WriteLine("1. Yape");
            Console.WriteLine("2. Tarjeta");
            Console.WriteLine("3. Cripto");
            Console.WriteLine("4. Plin");    // 👈 nueva opción
            Console.Write("Elige: ");

            string opcionPago = Console.ReadLine();

            // OCP/DIP: Delegar la creación del método de pago a la fábrica
            IMetodoPago metodoPago = pagoFactory.CrearMetodoPago(opcionPago);
            if (metodoPago == null)
            {
                Console.WriteLine("[ERROR] Método de pago inválido. ENTER para volver...");
                Console.ReadLine();
                return;
            }

            // ------------------------------------------------------------
            // 8. COBRAR Y DESCONTAR STOCK  👈 MODIFICADO
            // ------------------------------------------------------------
            Console.WriteLine();
            bool exito = metodoPago.Pagar(total);

            if (exito)
            {
                // Descontar de la sede elegida
                sedeElegida.ReducirStock(productoElegido, cantidad);

                Console.WriteLine($"[OK] Venta registrada con {metodoPago.Nombre}.");
                Console.WriteLine($"[INFO] Stock restante en {sedeElegida.Nombre}: {sedeElegida.Stock[productoElegido]}");
            }
            else
            {
                Console.WriteLine($"[ERROR] El pago con {metodoPago.Nombre} no se pudo completar.");
            }

            Console.WriteLine("Presiona ENTER para volver...");
            Console.ReadLine();
        }

        // ============ MÉTODO AUXILIAR DE FORMATO ============        
        static string FormatoSoles(decimal monto)
        {
            return $"S/ {monto:F2}";
        }

        // Formatea un monto según el tipo de moneda
        static string FormatearMoneda(decimal monto, TipoMoneda moneda)
        {
            if (moneda == TipoMoneda.PEN)
                return $"S/ {monto:F2}";

            if (moneda == TipoMoneda.BTC || moneda == TipoMoneda.ETH)
                return $"{monto:F8} {moneda}";

            return $"{monto:F2} {moneda}";
        }

    }
}