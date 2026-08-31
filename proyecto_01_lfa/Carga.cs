using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SimuladorAFD
{
    /// Funcionalidad 1: Carga
    /// Clase encargada de la extracción, parseo y construcción del AFD
    /// tanto desde un archivo de texto .txt como de forma interactiva.
    public class Carga
    {
        /// Lee un archivo .txt y extrae los componentes de la túpla usando Expresiones Regulares.
        public static AutomataFinito CargarDesdeArchivo(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"\nEl archivo en la ruta '{rutaArchivo}' no existe.");
            }

            AutomataFinito afd = new AutomataFinito();
            string[] lineas = File.ReadAllLines(rutaArchivo);

            bool procesandoTransiciones = false;

            // Patrones Regex para validar y extraer la sintaxis de cada componente
            Regex regexQ = new Regex(@"^Q\s*=\s*\{\s*(.*?)\s*\}$");
            Regex regexA = new Regex(@"^A\s*=\s*\{\s*(.*?)\s*\}$");
            Regex regexS = new Regex(@"^S\s*=\s*(\w+)$");
            Regex regexF = new Regex(@"^F\s*=\s*\{\s*(.*?)\s*\}$");
            Regex regexTransicion = new Regex(@"^\(\s*(\w+)\s*,\s*(.)\s*\)\s*->\s*(\w+),?$");

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();

                // Ignorar líneas vacías
                if (string.IsNullOrWhiteSpace(linea)) continue;

                // Si estamos dentro del bloque de transiciones T = { ... }
                if (procesandoTransiciones)
                {
                    if (linea == "}")
                    {
                        procesandoTransiciones = false;
                        continue;
                    }

                    Match matchT = regexTransicion.Match(linea);
                    if (matchT.Success)
                    {
                        string origen = matchT.Groups[1].Value;
                        char simbolo = matchT.Groups[2].Value[0];
                        string destino = matchT.Groups[3].Value;

                        afd.AgregarTransicion(origen, simbolo, destino);
                    }
                    else
                    {
                        throw new FormatException($"Error de sintaxis en la línea {i + 1} (Transición inválida): '{linea}'");
                    }
                    continue;
                }

                // Parseo de los conjuntos principales
                if (regexQ.IsMatch(linea))
                {
                    Match m = regexQ.Match(linea);
                    string[] estados = m.Groups[1].Value.Split(',');
                    foreach (var e in estados) afd.Estados.Add(e.Trim());
                }
                else if (regexA.IsMatch(linea))
                {
                    Match m = regexA.Match(linea);
                    string[] simbolos = m.Groups[1].Value.Split(',');
                    foreach (var s in simbolos)
                    {
                        string elem = s.Trim();
                        if (elem.Length > 0) afd.Alfabeto.Add(elem[0]);
                    }
                }
                else if (regexS.IsMatch(linea))
                {
                    Match m = regexS.Match(linea);
                    afd.EstadoInicial = m.Groups[1].Value.Trim();
                }
                else if (regexF.IsMatch(linea))
                {
                    Match m = regexF.Match(linea);
                    string[] finales = m.Groups[1].Value.Split(',');
                    foreach (var f in finales) afd.EstadosFinales.Add(f.Trim());
                }
                else if (linea.StartsWith("T = {"))
                {
                    procesandoTransiciones = true;
                }
                else
                {
                    throw new FormatException($"\nError de sintaxis en el archivo (línea {i + 1}): '{linea}'");
                }
            }

            return afd;
        }

        /// Solicita al usuario los elementos de la túpla paso a paso mediante la consola.
        public static AutomataFinito CargarInteractivamente()
        {
            AutomataFinito afd = new AutomataFinito();

            Console.WriteLine("\n--- INGRESO INTERACTIVO DEL AFD ---");

            // 1. Estados Q
            Console.WriteLine("\nIngrese los estados separados por coma (ej. q0, q1, q2): ");
            string[] estados = Console.ReadLine().Split(',');
            foreach (var e in estados) afd.Estados.Add(e.Trim());

            // 2. Alfabeto A (Σ)
            Console.WriteLine("\nIngrese el alfabeto separado por coma (ej. 0, 1): ");
            string[] simbolos = Console.ReadLine().Split(',');
            foreach (var s in simbolos)
            {
                string elem = s.Trim();
                if (elem.Length > 0) afd.Alfabeto.Add(elem[0]);
            }

            // 3. Estado Inicial S
            Console.WriteLine("\nIngrese el estado inicial (ej. q0): ");
            afd.EstadoInicial = Console.ReadLine().Trim();

            // 4. Estados Finales F
            Console.WriteLine("\nIngrese los estados finales separados por coma (ej. q2): ");
            string[] finales = Console.ReadLine().Split(',');
            foreach (var f in finales) afd.EstadosFinales.Add(f.Trim());

            // 5. Transiciones T
            Console.WriteLine("\nIngreso de transiciones (escribe 'fin' para terminar):");
            Console.WriteLine("\nFormato esperado: estadoOrigen, simbolo, estadoDestino (ej. q0, 0, q0)");

            while (true)
            {
                Console.WriteLine("\nTransición > ");
                string entrada = Console.ReadLine().Trim();
                if (entrada.ToLower() == "fin") break;

                string[] partes = entrada.Split(',');
                if (partes.Length == 3)
                {
                    string origen = partes[0].Trim();
                    char simbolo = partes[1].Trim()[0];
                    string destino = partes[2].Trim();

                    afd.AgregarTransicion(origen, simbolo, destino);
                }
                else
                {
                    Console.WriteLine(" Formato incorrecto. Intente de nuevo.");
                }
            }

            return afd;
        }
    }
}