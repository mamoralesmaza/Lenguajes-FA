using System;
using System.Collections.Generic;

/// Martín Andrés Morales Mazariegos 1217025
 ///Diego Alejandro Gualim Ramírez  1250025  


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

            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\n======================================================");
                Console.WriteLine("     SIMULADOR DE AUTÓMATAS FINITOS (AFD / AFND) ");
                Console.WriteLine("======================================================");
                Console.WriteLine("--- FASE 1: AUTÓMATA FINITO DETERMINISTA (AFD) ---");
                Console.WriteLine("1. Cargar AFD desde archivo (.txt)");
                Console.WriteLine("2. Ingresar AFD manualmente");
                Console.WriteLine("\n--- FASE 2: AUTÓMATA FINITO NO DETERMINISTA (AFND) ---");
                Console.WriteLine("3. Cargar AFND desde archivo (.txt) y transformar a AFD");
                Console.WriteLine("4. Ingresar AFND manualmente y transformar a AFD");
                Console.WriteLine("\n--- OPERACIONES SOBRE EL AFD ACTIVO EN MEMORIA ---");
                Console.WriteLine("5. Mostrar Tabla de Transición del AFD");
                Console.WriteLine("6. Evaluar una cadena individual");
                Console.WriteLine("7. Evaluar lote de cadenas (.txt)");
                Console.WriteLine("8. Reiniciar / Limpiar autómatas en memoria");
                Console.WriteLine("9. Salir");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine() ?? "";

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
                            // 1. Cargar AFND desde archivo
                            AFND afndTemp = CargaAFND.CargarDesdeArchivo(rutaAFND);
                            afndActual = afndTemp;

                            // 2. Mostrar la Tabla de Transición del AFND original (Funcionalidad 1)
                            TablaTransicionAFND.MostrarTabla(afndActual);

                            // 3. Transformación automática de AFND a AFD (Funcionalidad 2)
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
                            // 1. Cargar AFND interactivamente por consola
                            AFND afndManual = CargaAFND.CargarInteractivamente();
                            afndActual = afndManual;

                            // 2. Mostrar la Tabla de Transición del AFND original (Funcionalidad 1)
                            TablaTransicionAFND.MostrarTabla(afndActual);

                            // 3. Transformación automática de AFND a AFD (Funcionalidad 2)
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
                    // FASE 1 & FASE 2: Operaciones sobre el AFD Activo
                    // -------------------------------------------------------------
                    case "5":
                        if (AsegurarAFD(afdActual))
                        {
                            // Muestra la matriz del AFD resultante (o cargado)
                            TablaTransicion.MostrarTablaTransicion(afdActual);
                        }
                        break;

                    case "6":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la cadena a evaluar (o presione Enter para cadena vacía λ):");
                            string cadena = Console.ReadLine() ?? "";

                            // Evalúa la cadena paso a paso sobre el AFD
                            SimuladorAFD.EvaluarCadena(afdActual, cadena);
                        }
                        break;

                    case "7":
                        if (AsegurarAFD(afdActual))
                        {
                            Console.WriteLine("\nIngrese la ruta del archivo de cadenas (.txt):");
                            string rutaCadenas = (Console.ReadLine() ?? "").Trim();

                            // Evalúa un conjunto de cadenas desde un archivo plano
                            SimuladorAFD.ValidacionArchivo(afdActual, rutaCadenas);
                        }
                        break;

                    case "8":
                        afdActual = null;
                        afndActual = null;
                        Console.WriteLine("\n[INFO] Memoria del programa limpiada. No hay autómatas cargados.");
                        break;

                    case "9":
                        salir = true;
                        Console.WriteLine("\n¡Gracias por utilizar el simulador de autómatas!");
                        break;

                    default:
                        Console.WriteLine("\n[OPCIÓN INVÁLIDA] Seleccione una opción del 1 al 9.");
                        break;
                }
            }
        }

        /// Verifica que exista un AFD válido guardado en memoria antes de permitir la simulación o despliegue.
        private static bool AsegurarAFD(AutomataFinito afd)
        {
            if (afd == null)
            {
                Console.WriteLine("\n[¡ADVERTENCIA!] Debes cargar un AFD (Opciones 1 o 2) o transformar un AFND (Opciones 3 o 4) primero.");
                return false;
            }
            return true;
        }
    }
}