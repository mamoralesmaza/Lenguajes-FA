using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    class Program
    {
        static void Main(string[] args)
        {
            AutomataFinito afdActual = null;
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\n==============================================");
                Console.WriteLine("    SIMULADOR DE AUTÓMATAS FINITOS (AFD)     ");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Cargar túpla desde archivo (.txt)"); /// Funcionalidad 1
                Console.WriteLine("2. Ingresar túpla manualmente");    /// Funcionalidad 1
                Console.WriteLine("3. Mostrar Tabla de Transición");       /// Funcionalidad 3
                Console.WriteLine("4. Evaluar una cadena individual");     /// Funcionalidad 4
                Console.WriteLine("5. Evaluar las cadenas (.txt)");        /// Funcionalidad 4
                Console.WriteLine("6. Reiniciar / Limpiar autómata actual");
                Console.WriteLine("7. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("\nIngrese la ruta del archivo (.txt): ");
                        string ruta = Console.ReadLine().Trim();
                        try
                        {
                            // Llamada a Funcionalidad 1 (Carga)
                            AutomataFinito afdTemp = Carga.CargarDesdeArchivo(ruta);

                            // Llamada a Funcionalidad 2 (Validación)
                            if (Motor.ValidarIntegridad(afdTemp, out List<string> errores))
                            {
                                afdActual = afdTemp;
                                Console.WriteLine("\nAFD cargado y validado con éxito...");
                            }
                            else
                            {
                                Console.WriteLine("\nError de validación en la túpla del archivo:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError al leer el archivo: {ex.Message}");
                        }
                        break;

                    case "2":
                        try
                        {
                            // Llamada a Funcionalidad 1 (Carga Manual)
                            AutomataFinito afdManual = Carga.CargarInteractivamente();

                            // Llamada a Funcionalidad 2 (Validación)
                            if (Motor.ValidarIntegridad(afdManual, out List<string> errores))
                            {
                                afdActual = afdManual;
                                Console.WriteLine("\nAFD cargado manualmente y validado con éxito...");
                            }
                            else
                            {
                                Console.WriteLine("\nError de validación en la túpla ingresada:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError durante el ingreso manual: {ex.Message}");
                        }
                        break;

                    case "3":
                        if (AsegurarAFD(afdActual))
                        {
                            // Llamada a Funcionalidad 3 (Tabla de Transición)
                            TablaTransicion.MostrarTablaTransicion(afdActual);
                        }
                        break;

                    case "4":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la cadena a evaluar (o presione Enter para cadena vacía λ):");
                            string cadena = Console.ReadLine();

                            // Llamada a Funcionalidad 4 (Simulación)
                            SimuladorAFD.EvaluarCadena(afdActual, cadena);
                        }
                        break;

                    case "5":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la ruta del archivo (.txt):");
                            string ruta1 = Console.ReadLine().Trim();

                            // Llamada a Funcionalidad 4 (Simulación por Lote)
                            SimuladorAFD.ValidacionArchivo(afdActual, ruta1);
                        }
                        break;

                    case "6":
                        afdActual = null;
                        Console.WriteLine("\nAutómata reiniciado exitosamente...");
                        break;

                    case "7":
                        salir = true;
                        Console.WriteLine("\n¡Gracias por utilizar el simulador!");
                        break;

                    default:
                        Console.WriteLine("\nOpción no válida. Intente de nuevo...");
                        break;
                }
            }
        }

        /// Método auxiliar para asegurar que exista un autómata en memoria antes de invocar las funcionalidades 3 y 4.
        private static bool AsegurarAFD(AutomataFinito afd)
        {
            if (afd == null)
            {
                Console.WriteLine("\nDebes cargar y validar primero un autómata (Opción 1 o 2).");
                return false;
            }
            return true;
        }
    }
}