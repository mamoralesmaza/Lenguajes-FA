using System;
using System.Collections.Generic;
using System.IO;

namespace SimuladorAFD
{
    /// <summary>
    /// Motor encargado de la ejecución paso a paso de cadenas, procesamiento por lotes
    /// y despliegue visual de la quintaúpla y la tabla de transición.
    /// </summary>
    public class SimuladorAFD
    {
        /// <summary>
        /// Procesa una cadena de entrada paso a paso mostrando la traza completa de ejecución.
        /// </summary>
        /// <param name="afd">Autómata predeterminado a evaluar</param>
        /// <param name="cadena">Cadena a procesar</param>
        /// <returns>True si la cadena es aceptada, False de lo contrario</returns>
        public static bool EvaluarCadena(AutomataFinito afd, string cadena)
        {
            Console.WriteLine($"\n==========================================");
            Console.WriteLine($" TRAZA DE EJECUCIÓN PARA CADENA: \"{cadena}\"");
            Console.WriteLine($"==========================================");

            string estadoActual = afd.EstadoInicial;
            Console.WriteLine($"Estado Inicial: {estadoActual}");

            for (int i = 0; i < cadena.Length; i++)
            {
                char simbolo = cadena[i];

                // Verificar si el símbolo pertenece al alfabeto
                if (!afd.Alfabeto.Contains(simbolo))
                {
                    Console.WriteLine($" ERROR: El símbolo '{simbolo}' no pertenece al alfabeto Σ.");
                    Console.WriteLine("Veredicto: RECHAZADA (Símbolo no reconocido)");
                    return false;
                }

                // Obtener el siguiente estado a través de la función de transición
                if (afd.Transiciones.TryGetValue((estadoActual, simbolo), out string estadoSiguiente))
                {
                    Console.WriteLine($" Paso {i + 1}: Estado actual [{estadoActual}] --('{simbolo}')--> Siguiente estado [{estadoSiguiente}]");
                    estadoActual = estadoSiguiente;
                }
                else
                {
                    Console.WriteLine($" Paso {i + 1}: Sin transición para ({estadoActual}, '{simbolo}')");
                    Console.WriteLine("Veredicto: RECHAZADA (Transición indefinida)");
                    return false;
                }
            }

            // Verificación si el estado final pertenece a F
            bool esAceptada = afd.EstadosFinales.Contains(estadoActual);
            Console.WriteLine($"------------------------------------------");
            Console.WriteLine($"Estado final alcanzado: {estadoActual}");
            Console.WriteLine($"Veredicto Final: {(esAceptada ? "ACEPTADA (Pertenece al Lenguaje)" : "RECHAZADA (Estado no final)")}");
            Console.WriteLine($"==========================================\n");

            return esAceptada;
        }

        /// <summary>
        /// Evalúa un conjunto de cadenas contenidas en un archivo .txt, una por línea.
        /// </summary>
        public static void EvaluarLote(AutomataFinito afd, string rutaArchivoLote)
        {
            if (!File.Exists(rutaArchivoLote))
            {
                Console.WriteLine($" El archivo de lote '{rutaArchivoLote}' no existe.");
                return;
            }

            string[] cadenas = File.ReadAllLines(rutaArchivoLote);
            Console.WriteLine($"\n--- PROCESANDO LOTE DE {cadenas.Length} CADENAS ---");

            foreach (var cadena in cadenas)
            {
                EvaluarCadena(afd, cadena.Trim());
            }
        }

        /// <summary>
        /// Imprime en consola la definición formal del AFD y su correspondiente Tabla de Transición.
        /// </summary>
        public static void MostrarTablaTransicion(AutomataFinito afd)
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("        DEFINICIÓN FORMAL DEL AFD        ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Q = {{ {string.Join(", ", afd.Estados)} }}");
            Console.WriteLine($"Σ = {{ {string.Join(", ", afd.Alfabeto)} }}");
            Console.WriteLine($"q0 = {afd.EstadoInicial}");
            Console.WriteLine($"F = {{ {string.Join(", ", afd.EstadosFinales)} }}");

            Console.WriteLine("\n==========================================");
            Console.WriteLine("           TABLA DE TRANSICIÓN            ");
            Console.WriteLine("==========================================");

            // Imprimir encabezado del alfabeto
            Console.Write($"{"Estado",-12}");
            foreach (var simbolo in afd.Alfabeto)
            {
                Console.Write($"|  {simbolo,-6}");
            }
            Console.WriteLine("\n" + new string('-', 12 + afd.Alfabeto.Count * 9));

            // Imprimir filas por cada estado
            foreach (var estado in afd.Estados)
            {
                string prefijo = "";
                if (estado == afd.EstadoInicial) prefijo += "->";
                if (afd.EstadosFinales.Contains(estado)) prefijo += "*";

                string estadoFormateado = $"{prefijo}{estado}";
                Console.Write($"{estadoFormateado,-12}");

                foreach (var simbolo in afd.Alfabeto)
                {
                    if (afd.Transiciones.TryGetValue((estado, simbolo), out string destino))
                    {
                        Console.Write($"|  {destino,-6}");
                    }
                    else
                    {
                        Console.Write($"|  {"-",-6}");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("==========================================\n");
        }
    }
}