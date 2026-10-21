using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimuladorAFD
{
    /// Motor de ejecución y simulación con traza dinámica de la pila para un APD.
    /// Evaluador individual y por lotes con formato paso a paso por consola.
    public static class SimuladorAPD
    {
        /// Evalúa una cadena de entrada e imprime en consola la traza dinámica de la pila y el estado.
        public static bool EvaluarCadena(APD apd, string cadena)
        {
            cadena = cadena.Trim();

            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"     VALIDACIÓN APD PARA CADENA: \"{cadena}\"");
            Console.WriteLine($"==========================================");

            foreach (char simbolo in cadena)
            {
                if (!apd.AlfabetoEntrada.Contains(simbolo))
                {
                    Console.WriteLine($"[ERROR] El símbolo '{simbolo}' no pertenece al alfabeto de entrada.");
                    Console.WriteLine("Validación: RECHAZADA (símbolo no reconocido)");
                    return false;
                }
            }

            // Inicialización de la Pila con el símbolo inicial Z0
            Stack<char> pila = new Stack<char>();
            pila.Push(apd.SimboloInicialPila);

            string estadoActual = apd.EstadoInicial;
            int indiceEntrada = 0;
            int numeroPaso = 1;

            Console.WriteLine($"Estado Inicial: {estadoActual}");
            Console.WriteLine($"Pila Inicial:   {FormatearPila(pila)}\n");

            // Bucle principal de procesamiento de la cadena
            while (true)
            {
                // Verificar si la pila se quedó vacía
                if (pila.Count == 0)
                {
                    Console.WriteLine("------------------------------------------");
                    Console.WriteLine("[BLOQUEO] La pila quedó completamente vacía antes de completar la evaluación.");
                    Console.WriteLine("Validación: RECHAZADA");
                    return false;
                }

                char topePila = pila.Peek();
                char simboloEntrada = (indiceEntrada < cadena.Length) ? cadena[indiceEntrada] : '\0';

                // 1. Intentar buscar una transición directa consumiendo el símbolo de entrada
                TransicionAPD? transicion = null;
                bool consumioEntrada = false;

                if (indiceEntrada < cadena.Length)
                {
                    transicion = apd.Transiciones.FirstOrDefault(t =>
                        t.EstadoOrigen == estadoActual &&
                        t.SimboloEntrada == simboloEntrada &&
                        t.TopePila == topePila);

                    if (transicion != null)
                    {
                        consumioEntrada = true;
                    }
                }

                // 2. Si no hay transición con símbolo real, buscar una transición épsilon (e / ε)
                if (transicion == null)
                {
                    transicion = apd.Transiciones.FirstOrDefault(t =>
                        t.EstadoOrigen == estadoActual &&
                        (t.SimboloEntrada == 'e' || t.SimboloEntrada == 'ε') &&
                        t.TopePila == topePila);
                }

                // Si no existe ninguna transición posible (ni con entrada ni con épsilon), el autómata se detiene
                if (transicion == null)
                {
                    break;
                }

                // --- Ejecución de la Transición ---
                // a. Desapilar el tope de la pila
                pila.Pop();

                // b. Apilar la cadena resultante en orden inverso
                string apilar = transicion.CadenaApilar;
                if (apilar != "e" && apilar != "ε")
                {
                    for (int i = apilar.Length - 1; i >= 0; i--)
                    {
                        pila.Push(apilar[i]);
                    }
                }

                // c. Actualizar estado y avanzar cursor de entrada si aplicó
                char simboloLeido = consumioEntrada ? simboloEntrada : 'ε';
                string proximoEstado = transicion.EstadoDestino;

                if (consumioEntrada)
                {
                    indiceEntrada++;
                }

                // Imprimir el paso actual de la simulación
                Console.WriteLine($"Paso {numeroPaso++}:");
                Console.WriteLine($"  Entrada leída:       [{(simboloLeido == '\0' ? "FIN" : simboloLeido.ToString())}]");
                Console.WriteLine($"  Estado actual:       [{estadoActual}] -> Nuevo Estado: [{proximoEstado}]");
                Console.WriteLine($"  Desapilado (Tope):   [{topePila}]");
                Console.WriteLine($"  Apilado resultante: [{apilar}]");
                Console.WriteLine($"  Pila actual:         {FormatearPila(pila)}\n");

                estadoActual = proximoEstado;
            }

            // --- Veredicto Final ---
            // Criterio formal de aceptación: Cadena consumida en su totalidad y estado actual ∈ F
            bool entradaTotalmenteConsumida = (indiceEntrada == cadena.Length);
            bool esEstadoFinal = apd.EstadosFinales.Contains(estadoActual);

            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Estado final alcanzado: [{estadoActual}]");
            Console.WriteLine($"Símbolos procesados:   {indiceEntrada} de {cadena.Length}");

            if (entradaTotalmenteConsumida && esEstadoFinal)
            {
                Console.WriteLine("Validación: ACEPTADA (Pertenece al Lenguaje)");
                return true;
            }
            else
            {
                string razon = !entradaTotalmenteConsumida 
                    ? "Bloqueo por falta de transición para el símbolo de entrada" 
                    : "El estado final no pertenece a F";
                Console.WriteLine($"Validación: RECHAZADA ({razon})");
                return false;
            }
        }

        /// Evalúa las cadenas almacenadas una por línea en un archivo de texto.
        public static void ValidacionArchivo(APD apd, string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine($"\n[ERROR] El archivo '{rutaArchivo}' no existe.");
                return;
            }

            string[] cadenas = File.ReadAllLines(rutaArchivo);
            Console.WriteLine($"\n--- PROCESANDO LOTE APD ({cadenas.Length} CADENAS) ---");

            foreach (string cadena in cadenas)
            {
                EvaluarCadena(apd, cadena.Trim());
            }
        }

        /// Representa visualmente la pila de tope a fondo. Ejemplo: [ A | A | Z ]
        private static string FormatearPila(Stack<char> pila)
        {
            if (pila.Count == 0) return "[ VACA ]";
            return "[ " + string.Join(" | ", pila.ToArray()) + " ]";
        }

        /// Imprime la séptupla formal y la tabla de transiciones en formato matricial.
        public static void MostrarTablaTransicion(APD apd)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("     DEFINICIÓN Y TABLA DE TRANSICIÓN APD     ");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Q  = {{ {string.Join(", ", apd.Estados)} }}");
            Console.WriteLine($"Σ  = {{ {string.Join(", ", apd.AlfabetoEntrada)} }}");
            Console.WriteLine($"Γ  = {{ {string.Join(", ", apd.AlfabetoPila)} }}");
            Console.WriteLine($"q0 = {apd.EstadoInicial}");
            Console.WriteLine($"Z0 = {apd.SimboloInicialPila}");
            Console.WriteLine($"F  = {{ {string.Join(", ", apd.EstadosFinales)} }}\n");

            Console.WriteLine("REGLAS DE TRANSICIÓN δ(q, a, Z) -> (p, γ):");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("{0,-12} | {1,-10} | {2,-10} | {3,-15}", "Estado (q)", "Entrada(a)", "Tope (Z)", "Destino (p, γ)");
            Console.WriteLine("--------------------------------------------------");

            foreach (var t in apd.Transiciones)
            {
                string inicio = (t.EstadoOrigen == apd.EstadoInicial) ? "->" + t.EstadoOrigen : t.EstadoOrigen;
                if (apd.EstadosFinales.Contains(t.EstadoOrigen)) inicio = "*" + inicio;

                Console.WriteLine("{0,-12} | {1,-10} | {2,-10} | {3,-15}",
                    inicio,
                    t.SimboloEntrada,
                    t.TopePila,
                    $"({t.EstadoDestino}, {t.CadenaApilar})");
            }
            Console.WriteLine("--------------------------------------------------");
        }
    }
}