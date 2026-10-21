using System;
using System.Collections.Generic;

/// Martín Andrés Morales Mazariegos 1217025
/// Diego Alejandro Gualim Ramírez  1250025  

namespace SimuladorAFD
{
    class Program
    {
        static void Main(string[] args)
        {
            // Guarda el AFD activo (ya sea cargado directamente o generado desde un AFND)
            AutomataFinito afdActual = null;
            
            // Guarda el AFND activo cuando se utiliza la Fase 2
            AFND afndActual = null;

            // Guarda el APD activo cuando se utiliza la Fase 3
            APD apdActual = null;

            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\n===============================================================");
                Console.WriteLine("    SIMULADOR DE AUTÓMATAS FINITOS Y PILA (AFD / AFND / APD) ");
                Console.WriteLine("===============================================================");
                Console.WriteLine("--- FASE 1: AUTÓMATA FINITO DETERMINISTA (AFD) ---");
                Console.WriteLine("1. Cargar AFD desde archivo (.txt)");
                Console.WriteLine("2. Ingresar AFD manualmente");
                Console.WriteLine("\n--- FASE 2: AUTÓMATA FINITO NO DETERMINISTA (AFND) ---");
                Console.WriteLine("3. Cargar AFND desde archivo (.txt) y transformar a AFD");
                Console.WriteLine("4. Ingresar AFND manualmente y transformar a AFD");
                Console.WriteLine("\n--- FASE 3: AUTÓMATA DE PILA DETERMINISTA (APD) ---");
                Console.WriteLine("5. Cargar APD desde archivo (.txt)");
                Console.WriteLine("6. Ingresar APD manualmente");
                Console.WriteLine("\n--- OPERACIONES SOBRE EL APD ACTIVO EN MEMORIA ---");
                Console.WriteLine("7. Mostrar Tabla de Transición del APD");
                Console.WriteLine("8. Evaluar una cadena en el APD");
                Console.WriteLine("9. Evaluar lote de cadenas en el APD (.txt)");
                Console.WriteLine("\n--- OPERACIONES SOBRE EL AFD ACTIVO EN MEMORIA ---");
                Console.WriteLine("10. Mostrar Tabla de Transición del AFD");
                Console.WriteLine("11. Evaluar una cadena individual en el AFD");
                Console.WriteLine("12. Evaluar lote de cadenas en el AFD (.txt)");
                Console.WriteLine("\n--- GENERAL ---");
                Console.WriteLine("13. Reiniciar / Limpiar autómatas en memoria");
                Console.WriteLine("14. Salir");
                Console.Write("\nSeleccione una opción: ");

                string opcion = (Console.ReadLine() ?? "").Trim();

                switch (opcion)
                {
                    // -------------------------------------------------------------
                    // FASE 1: Carga directa de AFD
                    // -------------------------------------------------------------
                    case "1":
                        Console.WriteLine("\nIngrese la ruta del archivo (.txt): ");
                        string rutaAFD = (Console.ReadLine() ?? "").Trim();
                        try
                        {
                            AutomataFinito afdTemp = Carga.CargarDesdeArchivo(rutaAFD);

                            if (Motor.ValidarIntegridad(afdTemp, out List<string> errores))
                            {
                                afdActual = afdTemp;
                                afndActual = null; // Limpiamos el AFND previo
                                Console.WriteLine("\nAFD cargado y validado correctamente.");
                            }
                            else
                            {
                                Console.WriteLine("\nError de validación. La túpla del archivo no es un AFD válido:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError! No se pudo leer el archivo: {ex.Message}");
                        }
                        break;

                    case "2":
                        try
                        {
                            AutomataFinito afdManual = Carga.CargarInteractivamente();

                            if (Motor.ValidarIntegridad(afdManual, out List<string> errores))
                            {
                                afdActual = afdManual;
                                afndActual = null;
                                Console.WriteLine("\nAFD cargado manualmente y validado con éxito.");
                            }
                            else
                            {
                                Console.WriteLine("\nError de validación. La túpla ingresada no cumple las reglas de un AFD:");
                                foreach (var err in errores) Console.WriteLine($"   - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError! Ocurrió un fallo en la entrada manual: {ex.Message}");
                        }
                        break;

                    // -------------------------------------------------------------
                    // FASE 2: Carga de AFND + Transformación por Subconjuntos
                    // -------------------------------------------------------------
                    case "3":
                        Console.WriteLine("\nIngrese la ruta del archivo del AFND (.txt): ");
                        string rutaAFND = (Console.ReadLine() ?? "").Trim();
                        try
                        {
                            AFND afndTemp = CargaAFND.CargarDesdeArchivo(rutaAFND);
                            afndActual = afndTemp;

                            TablaTransicionAFND.MostrarTabla(afndActual);

                            Console.WriteLine("\nProcesando Algoritmo de Construcción de Subconjuntos...");
                            afdActual = TransformadorSubconjuntos.ConvertirAFNDaAFD(afndActual);

                            Console.WriteLine("AFND transformado a AFD equivalente y almacenado correctamente en la memoria.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError al procesar el AFND: {ex.Message}");
                        }
                        break;

                    case "4":
                        try
                        {
                            AFND afndManual = CargaAFND.CargarInteractivamente();
                            afndActual = afndManual;

                            TablaTransicionAFND.MostrarTabla(afndActual);

                            Console.WriteLine("\nProcesando Algoritmo de Construcción de Subconjuntos...");
                            afdActual = TransformadorSubconjuntos.ConvertirAFNDaAFD(afndActual);

                            Console.WriteLine("AFND transformado a AFD equivalente y almacenado correctamente en la memoria.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError durante el ingreso interactivo del AFND: {ex.Message}");
                        }
                        break;

                    // -------------------------------------------------------------
                    // FASE 3: Autómata de Pila Determinista (APD)
                    // -------------------------------------------------------------
                    case "5":
                        Console.WriteLine("\nIngrese la ruta del archivo del APD (.txt): ");
                        string rutaAPD = (Console.ReadLine() ?? "").Trim();
                        try
                        {
                            APD apdTemp = CargaAPD.CargarDesdeArchivo(rutaAPD);
                            if (apdTemp != null)
                            {
                                apdActual = apdTemp;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError al cargar el APD: {ex.Message}");
                        }
                        break;

                    case "6":
                        try
                        {
                            APD apdManual = CargaAPD.CargarInteractively();
                            if (apdManual != null)
                            {
                                apdActual = apdManual;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError durante la entrada manual del APD: {ex.Message}");
                        }
                        break;

                    case "7":
                        if (AsegurarAPD(apdActual))
                        {
                            SimuladorAPD.MostrarTablaTransicion(apdActual);
                        }
                        break;

                    case "8":
                        if (AsegurarAPD(apdActual))
                        {
                            Console.WriteLine("\nIngrese la cadena a evaluar en el APD:");
                            string cadenaAPD = Console.ReadLine() ?? "";
                            SimuladorAPD.EvaluarCadena(apdActual, cadenaAPD);
                        }
                        break;

                    case "9":
                        if (AsegurarAPD(apdActual))
                        {
                            Console.WriteLine("\nIngrese la ruta del archivo de cadenas para APD (.txt):");
                            string rutaLoteAPD = (Console.ReadLine() ?? "").Trim();
                            SimuladorAPD.ValidacionArchivo(apdActual, rutaLoteAPD);
                        }
                        break;

                    // -------------------------------------------------------------
                    // Operaciones sobre el AFD Activo
                    // -------------------------------------------------------------
                    case "10":
                        if (AsegurarAFD(afdActual))
                        {
                            TablaTransicion.MostrarTablaTransicion(afdActual);
                        }
                        break;

                    case "11":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la cadena a evaluar (o presione Enter para cadena vacía λ):");
                            string cadena = Console.ReadLine() ?? "";
                            SimuladorAFD.EvaluarCadena(afdActual, cadena);
                        }
                        break;

                    case "12":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la ruta del archivo de cadenas (.txt):");
                            string rutaCadenas = (Console.ReadLine() ?? "").Trim();
                            SimuladorAFD.ValidacionArchivo(afdActual, rutaCadenas);
                        }
                        break;

                    // -------------------------------------------------------------
                    // Limpieza y Salida
                    // -------------------------------------------------------------
                    case "13":
                        afdActual = null;
                        afndActual = null;
                        apdActual = null;
                        Console.WriteLine("\n[INFO] Memoria del programa limpiada. No hay autómatas cargados.");
                        break;

                    case "14":
                        salir = true;
                        Console.WriteLine("\n¡Gracias por utilizar el simulador de autómatas!");
                        break;

                    default:
                        Console.WriteLine("\n[OPCIÓN INVÁLIDA] Seleccione una opción del 1 al 14.");
                        break;
                }
            }
        }

        /// Verifica que exista un AFD válido guardado en memoria.
        private static bool AsegurarAFD(AutomataFinito afd)
        {
            if (afd == null)
            {
                Console.WriteLine("\n[¡ADVERTENCIA!] Debes cargar un AFD (Opciones 1 o 2) o transformar un AFND (Opciones 3 o 4) primero.");
                return false;
            }
            return true;
        }

        /// Verifica que exista un APD válido guardado en memoria.
        private static bool AsegurarAPD(APD apd)
        {
            if (apd == null)
            {
                Console.WriteLine("\n[¡ADVERTENCIA!] Debes cargar un APD (Opciones 5 o 6) primero.");
                return false;
            }
            return true;
        }

    }
}