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
using System.Diagnostics;
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

        // Variables globales: const y readonly de ejemplo
        const string APP_NAME = "SistemaVentasConsola"; // const inmutable en tiempo de compilación
        static readonly DateTime START_TIME = DateTime.UtcNow; // readonly en tiempo de ejecución

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

            // Crear repositorios usando las listas globales como backing store
            var usuarioRepo = new Repos.UsuarioRepository(usuarios);
            var productoRepo = new Repos.ProductoRepository(productos);
            var sedeRepo = new Repos.SedeRepository(sedes);

            // Crear UIManager y delegar todo el menú en la clase (Main solo ejecución)
            var ui = new UI.UIManager(usuarioRepo, productoRepo, sedeRepo);
            ui.Run();
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
        public static void RegistrarUsuario()
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
        public static void RegistrarProducto()
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

        // Genera 'cantidad' productos y los añade a la lista 'productos'
        static void GenerarProductos(int cantidad)
        {
            var nombres = new string[]
            {
                "Leche","Pan","Queso","Arroz","Huevos","Pollo","Manzana","Banana","Camiseta","Pantalon",
                "Zapatos","Laptop","Mouse","Teclado","Auriculares","Jugo","Cereal","Aceite","Azucar","Sal"
            };

            var alimentos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Leche","Pan","Queso","Arroz","Huevos","Pollo","Manzana","Banana","Jugo","Cereal","Aceite","Azucar","Sal"
            };

            var ropaSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Camiseta","Pantalon","Zapatos"
            };

            var tecnologiaSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Laptop","Mouse","Teclado","Auriculares"
            };

            var rnd = new Random(42);
            // Si ya hay productos, empezamos después del último índice para nombres únicos
            int start = productos.Count + 1;

            for (int i = start; i < start + cantidad; i++)
            {
                string baseNombre = nombres[(i - 1) % nombres.Length];
                string nombre = baseNombre + (i > nombres.Length ? i.ToString() : "");

                Categoria cat;
                if (alimentos.Contains(baseNombre)) cat = new Categorias.CategoriaAlimento();
                else if (ropaSet.Contains(baseNombre)) cat = new Categorias.CategoriaRopa();
                else if (tecnologiaSet.Contains(baseNombre)) cat = new Categorias.CategoriaTecnologia();
                else cat = new Categorias.CategoriaAlimento();

                decimal precio = (decimal)(rnd.NextDouble() * 500.0 + 1.0);
                productos.Add(new Producto(nombre, Math.Round(precio, 2), cat));
            }
        }

        // Mide en ms la ejecución de una acción (GC previo para mayor consistencia)
        static long MedirMs(Action accion)
        {
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            var sw = Stopwatch.StartNew();
            accion();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        // Ejecuta un conjunto de pruebas LINQ sobre 'productos' y muestra tiempos
        static void EjecutarPruebasLINQ(int cantidad)
        {
            Console.Clear();
            Console.WriteLine($"Generando {cantidad} productos para pruebas LINQ...");
            productos.Clear();
            GenerarProductos(cantidad);

            Console.WriteLine("Ejecutando pruebas (se muestran ms) ...\n");

            // WHERE
            long msWhereSeq = MedirMs(() => { var c = productos.Where(p => p.Precio > 100m).Count(); });
            long msWherePar = MedirMs(() => { var c = productos.AsParallel().Where(p => p.Precio > 100m).Count(); });
            Console.WriteLine($"WHERE  -> Seq: {msWhereSeq} ms | PLINQ: {msWherePar} ms");

            // SELECT
            long msSelectSeq = MedirMs(() => { var list = productos.Select(p => p.Nombre).ToList(); });
            long msSelectPar = MedirMs(() => { var list = productos.AsParallel().Select(p => p.Nombre).ToList(); });
            Console.WriteLine($"SELECT -> Seq: {msSelectSeq} ms | PLINQ: {msSelectPar} ms");

            // ORDERBY + Take(100)
            long msOrderSeq = MedirMs(() => { var top = productos.OrderBy(p => p.Precio).Take(100).ToList(); });
            long msOrderPar = MedirMs(() => { var top = productos.AsParallel().OrderBy(p => p.Precio).Take(100).ToList(); });
            Console.WriteLine($"ORDERBY (Take100) -> Seq: {msOrderSeq} ms | PLINQ: {msOrderPar} ms");

            // GROUPBY vs ToLookup
            long msGroupSeq = MedirMs(() => { var g = productos.GroupBy(p => p.Categoria.Nombre).Select(gp => new { Key = gp.Key, C = gp.Count() }).ToList(); });
            long msLookup = MedirMs(() => { var lu = productos.ToLookup(p => p.Categoria.Nombre); var x = lu["Alimento"].Count(); });
            Console.WriteLine($"GROUPBY -> Seq: {msGroupSeq} ms | ToLookup: {msLookup} ms");

            Console.WriteLine("\nMuestra primeros 20 productos generados:");
            foreach (var p in productos.Take(20))
                Console.WriteLine($"{p.Nombre} - {p.Categoria.Nombre} - S/ {p.Precio:F2}");

            Console.WriteLine("\nENTER para volver...");
            Console.ReadLine();
        }
        public static void VerProductos()
        {
            Console.Clear();
            Console.WriteLine("--- PRODUCTOS ---");
            // No se generan productos automáticamente aquí.
            // La generación masiva solo ocurre si el usuario lo solicita explícitamente
            // mediante la opción de pruebas LINQ dentro de este mismo método.

            // Preguntar si se desea generar muchos registros para probar LINQ
            Console.Write("\n¿Deseas generar muchos registros para probar LINQ? (s/N): ");
            var resp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(resp) && resp.Trim().ToLower().StartsWith("s"))
            {
                Console.Write("Cantidad de registros a generar (ej: 100000): ");
                if (int.TryParse(Console.ReadLine(), out int cant) && cant > 0)
                {
                    EjecutarPruebasLINQ(cant);
                }
            }

            Console.WriteLine("\n--- FIN EJEMPLOS LINQ ---\n");

            // ---------------------- LISTADO DETALLADO (existente) ----------------------
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

            Console.WriteLine("ENTER para volver...");
            Console.ReadLine();
        }

        // ============ SEDES ============
        public static void RegistrarSede()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("--- REGISTRAR SEDE ---");
                Console.Write("Nombre de la sede: ");
                string nombre = Console.ReadLine();
                sedes.Add(new Sede(nombre));
                Console.WriteLine("[OK] Sede registrada. ENTER...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Manejo centralizado: envolver en RepositoryException
                throw new Repos.RepositoryException("Error registrando sede", ex);
            }
        }

        public static void AgregarStock()
        {
            try
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
            catch (Exception ex)
            {
                throw new Repos.RepositoryException("Error agregando stock", ex);
            }
        }

        // ============ DIVISAS ============
        public static void ConvertirDivisas()
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
        public static void AccesoDenegado()
        {
            Console.WriteLine("[ERROR] No tienes permisos para esta opción.");
            Console.WriteLine("Presiona ENTER para volver...");
            Console.ReadLine();
        }

        // ============ REGISTRAR VENTA ============
        public static void RegistrarVenta()
        {
            try
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
                // 1. ELEGIR SEDE
                // ------------------------------------------------------------
                Console.WriteLine("\n--- SELECCIONA LA SEDE ---");
                for (int i = 0; i < sedes.Count; i++)
                    Console.WriteLine($"{i + 1}. {sedes[i].Nombre}");

                Console.Write("\nElige sede: ");
                if (!int.TryParse(Console.ReadLine(), out int numeroSede) ||
                    numeroSede < 1 || numeroSede > sedes.Count)
                {
                    Console.WriteLine("[ERROR] Sede inválida. ENTER para volver...");
                    Console.ReadLine();
                    return;
                }

                Sede sedeElegida = sedes[numeroSede - 1];

                // ------------------------------------------------------------
                // 2. MOSTRAR PRODUCTOS CON STOCK DE ESA SEDE
                // ------------------------------------------------------------
                Console.WriteLine($"\n--- PRODUCTOS EN {sedeElegida.Nombre.ToUpper()} ---");
                for (int i = 0; i < productos.Count; i++)
                {
                    int stockEnSede = sedeElegida.Stock.ContainsKey(productos[i]) ? sedeElegida.Stock[productos[i]] : 0;
                    Console.WriteLine($"{i + 1}. {productos[i].Nombre} - {ConsoleApp_17_09_2026.Util.Formatos.FormatoSoles(productos[i].PrecioConImpuesto())} | Stock: {stockEnSede}");
                }

                Console.Write("\nElige producto: ");
                if (!int.TryParse(Console.ReadLine(), out int numeroProducto) ||
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
                int stockDisponible = sedeElegida.Stock.ContainsKey(productoElegido) ? sedeElegida.Stock[productoElegido] : 0;
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
                if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
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
                // 5. DETERMINAR CLIENTE (siempre preguntar: incluye Consumidor Final)
                // ------------------------------------------------------------
                List<Usuario> clientes = usuarios.FindAll(u => u.Rol == RolUsuario.Cliente);

                Console.WriteLine();
                Console.WriteLine("--- CLIENTE DE LA VENTA ---");
                Console.WriteLine("0. Consumidor Final");
                if (clientes.Count == 0)
                {
                    Console.WriteLine("(No hay clientes registrados)");
                }
                else
                {
                    for (int i = 0; i < clientes.Count; i++)
                        Console.WriteLine($"{i + 1}. {clientes[i].Nombre}");
                }

                Console.Write("Elige cliente (número): ");
                if (!int.TryParse(Console.ReadLine(), out int numCliente))
                {
                    Console.WriteLine("[ERROR] Entrada inválida. ENTER para volver...");
                    Console.ReadLine();
                    return;
                }

                Usuario clienteVenta;
                if (numCliente == 0)
                {
                    clienteVenta = new Usuario("Consumidor Final", "-", RolUsuario.Cliente);
                }
                else if (numCliente >= 1 && numCliente <= clientes.Count)
                {
                    clienteVenta = clientes[numCliente - 1];
                }
                else
                {
                    Console.WriteLine("[ERROR] Cliente inválido. ENTER para volver...");
                    Console.ReadLine();
                    return;
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
                Console.WriteLine($"Vendedor: {(usuarioActual == null ? "(no identificado)" : usuarioActual.Nombre)}");
                Console.WriteLine($"Producto: {productoElegido.Nombre}");
                Console.WriteLine($"Categoría: {productoElegido.Categoria.Nombre}");
                Console.WriteLine($"Precio unitario: {ConsoleApp_17_09_2026.Util.Formatos.FormatoSoles(precioUnitario)}");
                Console.WriteLine($"Cantidad: {cantidad}");
                Console.WriteLine($"Total a pagar: {ConsoleApp_17_09_2026.Util.Formatos.FormatoSoles(total)}");
                Console.WriteLine("=============================");

                // ------------------------------------------------------------
                // 7. ELEGIR MÉTODO DE PAGO
                // ------------------------------------------------------------
                Console.WriteLine();
                Console.WriteLine("--- MÉTODO DE PAGO ---");
                Console.WriteLine("1. Yape");
                Console.WriteLine("2. Tarjeta");
                Console.WriteLine("3. Cripto");
                Console.WriteLine("4. Plin");
                Console.Write("Elige: ");

                string opcionPago = Console.ReadLine();
                IMetodoPago metodoPago = pagoFactory.CrearMetodoPago(opcionPago);
                if (metodoPago == null)
                {
                    Console.WriteLine("[ERROR] Método de pago inválido. ENTER para volver...");
                    Console.ReadLine();
                    return;
                }

                // ------------------------------------------------------------
                // 8. COBRAR Y DESCONTAR STOCK
                // ------------------------------------------------------------
                Console.WriteLine();
                bool exito = metodoPago.Pagar(total);

                if (exito)
                {
                    sedeElegida.ReducirStock(productoElegido, cantidad);
                    int restante = sedeElegida.Stock.ContainsKey(productoElegido) ? sedeElegida.Stock[productoElegido] : 0;
                    Console.WriteLine($"[OK] Venta registrada con {metodoPago.Nombre}.");
                    Console.WriteLine($"[INFO] Stock restante en {sedeElegida.Nombre}: {restante}");
                }
                else
                {
                    Console.WriteLine($"[ERROR] El pago con {metodoPago.Nombre} no se pudo completar.");
                }

                Console.WriteLine("Presiona ENTER para volver...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw new Repos.RepositoryException("Error registrando venta", ex);
            }
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