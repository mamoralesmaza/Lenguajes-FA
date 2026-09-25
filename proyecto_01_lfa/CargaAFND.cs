using System;
using System.Collections.Generic;
using System.IO;

namespace SimuladorAFD
{
    /// Clase encargada de la lectura y construcción de objetos AFND 
    /// desde archivos planos (.txt) o mediante consola interactiva.
    public static class CargaAFND
    {
        /// Lee un archivo .txt y extrae la quintúpla del AFND.
        /// Suporta multiples estados destino en las transiciones (separados por coma).
        /// <param name="rutaArchivo">Ruta o nombre del archivo .txt en la carpeta del ejecutable</param>
        /// <returns>Objeto AFND cargado</returns>
        public static AFND CargarDesdeArchivo(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"\nError! El archivo no existe en la ruta: {rutaArchivo}");
            }

            AFND afnd = new AFND();
            string[] lineas = File.ReadAllLines(rutaArchivo);
            bool procesandoTransiciones = false;

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();

                // Ignorar líneas vacías o comentarios que inicien con //
                if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith("//"))
                    continue;

                // Detectar el inicio del bloque de transiciones T = {
                if (linea.StartsWith("T = {"))
                {
                    procesandoTransiciones = true;
                    continue;
                }

                // Si se encuentra la llave de cierre del bloque de transiciones
                if (procesandoTransiciones && linea == "}")
                {
                    procesandoTransiciones = false;
                    continue;
                }

                if (!procesandoTransiciones)
                {
                    // Parsing de los conjuntos principales (Q, A, S, F)
                    if (linea.StartsWith("Q ="))
                    {
                        string contenido = ExtraerContenidoEntreLlaves(linea);
                        string[] estados = contenido.Split(',');
                        foreach (var e in estados)
                            if (!string.IsNullOrWhiteSpace(e)) afnd.Estados.Add(e.Trim());
                    }
                    else if (linea.StartsWith("A =") || linea.StartsWith("Σ ="))
                    {
                        string contenido = ExtraerContenidoEntreLlaves(linea);
                        string[] simbolos = contenido.Split(',');
                        foreach (var s in simbolos)
                        {
                            string elem = s.Trim();
                            if (elem.Length > 0) afnd.Alfabeto.Add(elem[0]);
                        }
                    }
                    else if (linea.StartsWith("S ="))
                    {
                        string[] partes = linea.Split('=');
                        if (partes.Length == 2) afnd.EstadoInicial = partes[1].Trim();
                    }
                    else if (linea.StartsWith("F ="))
                    {
                        string contenido = ExtraerContenidoEntreLlaves(linea);
                        string[] finales = contenido.Split(',');
                        foreach (var f in finales)
                            if (!string.IsNullOrWhiteSpace(f)) afnd.EstadosFinales.Add(f.Trim());
                    }
                }
                else
                {
                    // Parsing de reglas de transición del AFND
                    // Formato esperado: (q0, 0) -> q0, q1
                    ProcesarLineaTransicion(linea, afnd);
                }
            }

            return afnd;
        }

        /// Permite el ingreso interactivo paso a paso de un AFND desde la consola.
        public static AFND CargarInteractivamente()
        {
            AFND afnd = new AFND();

            Console.WriteLine("\n===== INGRESO INTERACTIVO DE AFND =====");

            // 1. Estados Q
            Console.WriteLine("\nIngrese los estados separados por coma (ej. q0, q1, q2): ");
            string[] estados = (Console.ReadLine() ?? "").Split(',');
            foreach (var e in estados)
                if (!string.IsNullOrWhiteSpace(e)) afnd.Estados.Add(e.Trim());

            // 2. Alfabeto A
            Console.WriteLine("\nIngrese el alfabeto separado por coma (ej. 0, 1): ");
            string[] simbolos = (Console.ReadLine() ?? "").Split(',');
            foreach (var s in simbolos)
            {
                string elem = s.Trim();
                if (elem.Length > 0) afnd.Alfabeto.Add(elem[0]);
            }

            // 3. Estado Inicial S
            Console.WriteLine("\nIngrese el estado inicial (ej. q0): ");
            afnd.EstadoInicial = (Console.ReadLine() ?? "").Trim();

            // 4. Estados Finales F
            Console.WriteLine("\nIngrese los estados finales separados por coma (ej. q2): ");
            string[] finales = (Console.ReadLine() ?? "").Split(',');
            foreach (var f in finales)
                if (!string.IsNullOrWhiteSpace(f)) afnd.EstadosFinales.Add(f.Trim());

            // 5. Transiciones T
            Console.WriteLine("\nIngreso de transiciones para AFND (escribe 'fin' para terminar):");
            Console.WriteLine("Formato esperado: origen, símbolo, destinos (ej. q0, 0, q0,q1)");

            while (true)
            {
                Console.Write("Transición > ");
                string entrada = (Console.ReadLine() ?? "").Trim();
                if (entrada.ToLower() == "fin") break;

                string[] partes = entrada.Split(',');
                if (partes.Length >= 3)
                {
                    string origen = partes[0].Trim();
                    char simbolo = partes[1].Trim()[0];

                    // Todos los elementos a partir del índice 2 son estados destino
                    for (int i = 2; i < partes.Length; i++)
                    {
                        string destino = partes[i].Trim();
                        if (!string.IsNullOrEmpty(destino))
                            afnd.AgregarTransicion(origen, simbolo, destino);
                    }
                }
                else
                {
                    Console.WriteLine("\n Error! Formato incorrecto. Debe ingresar: origen, símbolo, destino(s).");
                }
            }

            return afnd;
        }

        /// Procesa una línea individual de transición con formato: (origen, simbolo) -> dest1, dest2
        private static void ProcesarLineaTransicion(string linea, AFND afnd)
        {
            // Remover comas al final de la línea si existen
            linea = linea.TrimEnd(',');

            string[] partes = linea.Split(new string[] { "->" }, StringSplitOptions.None);
            if (partes.Length != 2) return;

            // Extraer (origen, simbolo)
            string izquierda = partes[0].Replace("(", "").Replace(")", "").Trim();
            string[] origSimb = izquierda.Split(',');

            if (origSimb.Length != 2) return;

            string origen = origSimb[0].Trim();
            char simbolo = origSimb[1].Trim()[0];

            // Extraer estados destino (pueden ser varios separados por coma)
            string derecha = partes[1].Trim();
            string[] destinos = derecha.Split(',');

            foreach (var d in destinos)
            {
                string destLimpio = d.Trim();
                if (!string.IsNullOrEmpty(destLimpio))
                {
                    afnd.AgregarTransicion(origen, simbolo, destLimpio);
                }
            }
        }

        /// Método auxiliar para extraer el texto que se encuentra dentro de las llaves { }
        private static string ExtraerContenidoEntreLlaves(string texto)
        {
            int inicio = texto.IndexOf('{');
            int fin = texto.LastIndexOf('}');
            if (inicio != -1 && fin != -1 && fin > inicio)
            {
                return texto.Substring(inicio + 1, fin - inicio - 1);
            }
            return "";
        }
    }
}