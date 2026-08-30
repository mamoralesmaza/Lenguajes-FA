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
                Console.WriteLine("1. Cargar quintúpla desde archivo (.txt)");
                Console.WriteLine("2. Ingresar quintúpla manualmente");
                Console.WriteLine("3. Mostrar definición formal y Tabla de Transición");
                Console.WriteLine("4. Evaluar una cadena individual");
                Console.WriteLine("5. Evaluar lote de cadenas desde archivo (.txt)");
                Console.WriteLine("6. Reiniciar / Limpiar autómata actual");
                Console.WriteLine("7. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Ingrese la ruta del archivo (.txt): ");
                        string ruta = Console.ReadLine().Trim();
                        try
                        {
                            AutomataFinito afdTemp = AFD.CargarDesdeArchivo(ruta);
                            if (ValidadorAFD.ValidarIntegridad(afdTemp, out List<string> errores))
                            {
                                afdActual = afdTemp;
                                Console.WriteLine(" AFD cargado y validado con éxito.");
                            }
                            else
                            {
                                Console.WriteLine(" Error de validación en la quintúpla del archivo:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($" Error al leer el archivo: {ex.Message}");
                        }
                        break;

                    case "2":
                        try
                        {
                            AutomataFinito afdManual = AFD.CargarInteractivamente();
                            if (ValidadorAFD.ValidarIntegridad(afdManual, out List<string> errores))
                            {
                                afdActual = afdManual;
                                Console.WriteLine(" AFD cargado manualmente y validado con éxito.");
                            }
                            else
                            {
                                Console.WriteLine(" Error de validación en la quintúpla ingresada:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($" Error durante el ingreso manual: {ex.Message}");
                        }
                        break;

                    case "3":
                        if (AsegurarAFD(afdActual))
                        {
                            SimuladorAFD.MostrarTablaTransicion(afdActual);
                        }
                        break;

                    case "4":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.Write("Ingrese la cadena a evaluar (o presione Enter para cadena vacía λ): ");
                            string cadena = Console.ReadLine();
                            SimuladorAFD.EvaluarCadena(afdActual, cadena);
                        }
                        break;

                    case "5":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.Write("Ingrese la ruta del archivo de lote (.txt): ");
                            string rutaLote = Console.ReadLine().Trim();
                            SimuladorAFD.EvaluarLote(afdActual, rutaLote);
                        }
                        break;

                    case "6":
                        afdActual = null;
                        Console.WriteLine(" Autómata reiniciado correctamente.");
                        break;

                    case "7":
                        salir = true;
                        Console.WriteLine("¡Gracias por utilizar el simulador!");
                        break;

                    default:
                        Console.WriteLine(" Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }

        private static bool AsegurarAFD(AutomataFinito afd)
        {
            if (afd == null)
            {
                Console.WriteLine(" Debe cargar primero un autómata (Opción 1 o 2).");
                return false;
            }
            return true;
        }
    }
}