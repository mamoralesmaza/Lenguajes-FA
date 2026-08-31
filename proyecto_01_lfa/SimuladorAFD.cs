using System;
using System.Collections.Generic;
using System.IO;

namespace SimuladorAFD
{
    /// Funcionalidad 4: Motor de evaluación y simulación paso a paso
    /// Motor encargado de la ejecución paso a paso de cadenas, procesamiento por lotes
    /// y despliegue visual de la túpla y la tabla de transición.
    public class SimuladorAFD
    {
        /// Procesa una cadena de entrada paso a paso mostrando la traza completa de ejecución.
        public static bool EvaluarCadena(AutomataFinito afd, string cadena)
        {
            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"     VALIDACION PARA CADENA: \"{cadena}\"");
            Console.WriteLine($"==========================================");

            string estadoActual = afd.EstadoInicial;
            Console.WriteLine($"Estado Inicial: {estadoActual}");

            for (int i = 0; i < cadena.Length; i++)
            {
                char simbolo = cadena[i];

                // Verificar si el símbolo pertenece al alfabeto
                if (!afd.Alfabeto.Contains(simbolo))
                {
                    Console.WriteLine($"\nERROR: El símbolo '{simbolo}' no pertenece al alfabeto Σ.");
                    Console.WriteLine("\nFALLO! (Símbolo no reconocido)");
                    return false;
                }

                // Obtener el siguiente estado a través de la función de transición
                if (afd.Transiciones.TryGetValue((estadoActual, simbolo), out string estadoSiguiente))
                {
                    Console.WriteLine($"\nPaso {i + 1}: Estado actual [{estadoActual}] --('{simbolo}')--> Siguiente estado [{estadoSiguiente}]");
                    estadoActual = estadoSiguiente;
                }
                else
                {
                    Console.WriteLine($"\nPaso {i + 1}: Sin transición para ({estadoActual}, '{simbolo}')");
                    Console.WriteLine("\nValidacion: RECHAZADA (Transición indefinida)");
                    return false;
                }
            }

            // Verificación si el estado final pertenece a F
            bool esAceptada = afd.EstadosFinales.Contains(estadoActual);
            Console.WriteLine($"------------------------------------------");
            Console.WriteLine($"Estado final alcanzado: {estadoActual}");
            Console.WriteLine($"Validacion: {(esAceptada ? "ACEPTADA (Pertenece al Lenguaje)" : "RECHAZADA (Estado final no congruente)")}");
            Console.WriteLine($"==========================================\n");

            return esAceptada;
        }

        /// Evalúa un conjunto de cadenas contenidas en un archivo .txt, una por línea.
        public static void ValidacionArchivo(AutomataFinito afd, string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine($"El archivo '{rutaArchivo}' no existe.");
                return;
            }

            string[] cadenas = File.ReadAllLines(rutaArchivo);
            Console.WriteLine($"\n--- PROCESANDO CADENAS ({cadenas.Length}) ---");

            foreach (var cadena in cadenas)
            {
                EvaluarCadena(afd, cadena.Trim());
            }
        }

        /// Imprime en consola la definición formal del AFD y su correspondiente Tabla de Transición.
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