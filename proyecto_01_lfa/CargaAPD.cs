using System;
using System.Collections.Generic;
using System.IO;

namespace SimuladorAFD
{
    /// Clase encargada de la lectura, parsing y validación de sintaxis de Autómatas de Pila Deterministas (APD)
    /// tanto desde archivos planos (.txt) como mediante entrada manual por consola.
    public static class CargaAPD
    {
        /// Lee un archivo .txt con la tupla del APD, parsea su estructura y valida su determinismo.
        public static APD? CargarDesdeArchivo(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine($"\n [ERROR] El archivo '{rutaArchivo}' no existe.");
                return null;
            }

            APD apd = new APD();
            string[] lineas = File.ReadAllLines(rutaArchivo);
            bool leyendoTransiciones = false;

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();

                // Ignorar líneas vacías o comentarios
                if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith("//"))
                    continue;

                try
                {
                    // Manejo del bloque de transiciones T = { ... }
                    if (linea.StartsWith("T = {") || linea.StartsWith("T={"))
                    {
                        leyendoTransiciones = true;
                        continue;
                    }

                    if (leyendoTransiciones)
                    {
                        if (linea.Contains("}"))
                        {
                            leyendoTransiciones = false;
                            continue;
                        }

                        // Procesar línea de transición: (q0, a, Z) -> (q0, AZ),
                        ProcesarLineaTransicion(linea, apd);
                        continue;
                    }

                    // Parsing de las líneas de la tupla
                    if (linea.StartsWith("Q =") || linea.StartsWith("Q="))
                    {
                        string contenido = ExtraerEntreLlaves(linea);
                        foreach (var e in contenido.Split(','))
                            if (!string.IsNullOrWhiteSpace(e)) apd.Estados.Add(e.Trim());
                    }
                    else if (linea.StartsWith("A =") || linea.StartsWith("A="))
                    {
                        string contenido = ExtraerEntreLlaves(linea);
                        foreach (var s in contenido.Split(','))
                        {
                            string elem = s.Trim();
                            if (elem.Length > 0) apd.AlfabetoEntrada.Add(elem[0]);
                        }
                    }
                    else if (linea.StartsWith("R =") || linea.StartsWith("R="))
                    {
                        string contenido = ExtraerEntreLlaves(linea);
                        foreach (var r in contenido.Split(','))
                        {
                            string elem = r.Trim();
                            if (elem.Length > 0) apd.AlfabetoPila.Add(elem[0]);
                        }
                    }
                    else if (linea.StartsWith("S =") || linea.StartsWith("S="))
                    {
                        apd.EstadoInicial = linea.Split('=')[1].Trim();
                    }
                    else if (linea.StartsWith("F =") || linea.StartsWith("F="))
                    {
                        string contenido = ExtraerEntreLlaves(linea);
                        foreach (var f in contenido.Split(','))
                            if (!string.IsNullOrWhiteSpace(f)) apd.EstadosFinales.Add(f.Trim());
                    }
                    else if (linea.StartsWith("Z0 =") || linea.StartsWith("Z0=") || linea.StartsWith("ZO =") || linea.StartsWith("ZO="))
                    {
                        string valor = linea.Split('=')[1].Trim();
                        if (valor.Length > 0) apd.SimboloInicialPila = valor[0];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n [ERROR DE SINTAXIS] En la línea {i + 1}: '{linea}'. Detalle: {ex.Message}");
                    return null;
                }
            }

            // Validar la condición de determinismo
            var (esValido, mensajeError) = apd.ValidarDeterminismo();
            if (!esValido)
            {
                Console.WriteLine($"\n [RECHAZADO] {mensajeError}");
                return null;
            }

            Console.WriteLine($"\n [ÉXITO] Archivo cargado correctamente. {mensajeError}");
            return apd;
        }

        /// Permite el ingreso interactivo de la tupla de un APD mediante la consola.
        public static APD? CargarInteractively()
        {
            APD apd = new APD();
            Console.WriteLine("\n--- INGRESO INTERACTIVO DE APD ---");

            // 1. Estados Q
            Console.Write("\nIngrese los estados Q separados por coma (ej. q0, q1, q2): ");
            foreach (var e in (Console.ReadLine() ?? "").Split(','))
                if (!string.IsNullOrWhiteSpace(e)) apd.Estados.Add(e.Trim());

            // 2. Alfabeto de entrada Σ (A)
            Console.Write("Ingrese el alfabeto de entrada Σ separado por coma (ej. a, b): ");
            foreach (var s in (Console.ReadLine() ?? "").Split(','))
                if (s.Trim().Length > 0) apd.AlfabetoEntrada.Add(s.Trim()[0]);

            // 3. Alfabeto de pila Γ (R)
            Console.Write("Ingrese el alfabeto de la pila Γ separado por coma (ej. A, Z): ");
            foreach (var r in (Console.ReadLine() ?? "").Split(','))
                if (r.Trim().Length > 0) apd.AlfabetoPila.Add(r.Trim()[0]);

            // 4. Estado Inicial S
            Console.Write("Ingrese el estado inicial S (ej. q0): ");
            apd.EstadoInicial = (Console.ReadLine() ?? "").Trim();

            // 5. Símbolo Inicial de Pila Z0
            Console.Write("Ingrese el símbolo inicial de la pila Z0 (ej. Z): ");
            string z0Input = (Console.ReadLine() ?? "").Trim();
            if (z0Input.Length > 0) apd.SimboloInicialPila = z0Input[0];

            // 6. Estados Finales F
            Console.Write("Ingrese los estados finales F separados por coma (ej. q2): ");
            foreach (var f in (Console.ReadLine() ?? "").Split(','))
                if (!string.IsNullOrWhiteSpace(f)) apd.EstadosFinales.Add(f.Trim());

            // 7. Transiciones T
            Console.WriteLine("\nIngreso de transiciones para APD (escribe 'fin' para terminar):");
            Console.WriteLine("Formato esperado: (origen, entrada, tope) -> (destino, apilar)");
            Console.WriteLine("Ejemplo: (q0, a, Z) -> (q0, AZ)");

            while (true)
            {
                Console.Write("Transición > ");
                string entrada = (Console.ReadLine() ?? "").Trim();
                if (entrada.ToLower() == "fin" || string.IsNullOrWhiteSpace(entrada)) break;

                try
                {
                    ProcesarLineaTransicion(entrada, apd);
                }
                catch
                {
                    Console.WriteLine(" Formato incorrecto. Ejemplo de uso: (q0, a, Z) -> (q0, AZ)");
                }
            }

            // Validar determinismo tras la entrada manual
            var (esValido, mensajeError) = apd.ValidarDeterminismo();
            if (!esValido)
            {
                Console.WriteLine($"\n [RECHAZADO] {mensajeError}");
                Console.WriteLine("El APD ingresado no se activó. Corrija las transiciones en un nuevo ingreso.");
                return null;
            }

            return apd;
        }

        #region Métodos Auxiliares de Parsing

        /// Parsea líneas con formato: (q0, a, Z) -> (q0, AZ)
        private static void ProcesarLineaTransicion(string linea, APD apd)
        {
            // Limpiar comas o llaves de cierre al final si viene dentro del bloque T
            linea = linea.TrimEnd(',', '}').Trim();
            if (string.IsNullOrWhiteSpace(linea)) return;

            string[] partes = linea.Split("->");
            if (partes.Length != 2) throw new FormatException("Falta el operador '->'");

            // Izquierda: (q0, a, Z)
            string izq = partes[0].Replace("(", "").Replace(")", "").Trim();
            string[] elementosIzq = izq.Split(',');
            if (elementosIzq.Length != 3) throw new FormatException("Lado izquierdo debe tener 3 elementos");

            string origen = elementosIzq[0].Trim();
            char entrada = elementosIzq[1].Trim()[0];
            char tope = elementosIzq[2].Trim()[0];

            // Derecha: (q0, AZ)
            string der = partes[1].Replace("(", "").Replace(")", "").Trim();
            string[] elementosDer = der.Split(',');
            if (elementosDer.Length != 2) throw new FormatException("Lado derecho debe tener 2 elementos");

            string destino = elementosDer[0].Trim();
            string apilar = elementosDer[1].Trim();

            apd.AgregarTransicion(origen, entrada, tope, destino, apilar);
        }

        /// Extrae el texto encerrado entre llaves { ... }
        private static string ExtraerEntreLlaves(string linea)
        {
            int inicio = linea.IndexOf('{');
            int fin = linea.LastIndexOf('}');
            if (inicio != -1 && fin != -1 && fin > inicio)
            {
                return linea.Substring(inicio + 1, fin - inicio - 1);
            }
            return "";
        }

        #endregion
    }
}