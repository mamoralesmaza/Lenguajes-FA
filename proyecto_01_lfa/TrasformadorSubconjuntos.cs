using System;
using System.Collections.Generic;
using System.Linq;

namespace SimuladorAFD
{
    /// Clase encargada de ejecutar el Algoritmo de Construcción de Subconjuntos.
    /// Preserva el nombre del estado inicial (ej. "q0") y asigna A, B, C... a los subsecuentes.
    public static class TransformadorSubconjuntos
    {
        public static AutomataFinito ConvertirAFNDaAFD(AFND afnd)
        {
            AutomataFinito afd = new AutomataFinito();
            afd.Alfabeto = new HashSet<char>(afnd.Alfabeto);

            Dictionary<string, HashSet<string>> mapaEquivalencias = new Dictionary<string, HashSet<string>>();
            Queue<HashSet<string>> colaPendientes = new Queue<HashSet<string>>();

            char letraActual = 'A';
            string nombreEstadoTrampa = null;

            // 1. El estado inicial conserva el nombre original del q0 del AFND
            HashSet<string> conjuntoInicial = new HashSet<string> { afnd.EstadoInicial };
            string nombreInicial = afnd.EstadoInicial; // Se mantiene "q0" sin cambiar a "A"

            mapaEquivalencias[nombreInicial] = conjuntoInicial;
            afd.Estados.Add(nombreInicial);
            afd.EstadoInicial = nombreInicial;

            colaPendientes.Enqueue(conjuntoInicial);

            // 2. Bucle principal de construcción de subconjuntos
            while (colaPendientes.Count > 0)
            {
                HashSet<string> conjuntoActual = colaPendientes.Dequeue();
                string nombreMacroActual = ObtenerNombreMacroEstado(conjuntoActual, mapaEquivalencias);

                foreach (char simbolo in afnd.Alfabeto)
                {
                    HashSet<string> conjuntoDestino = new HashSet<string>();

                    foreach (string estadoAFND in conjuntoActual)
                    {
                        var clave = (estadoAFND, simbolo);
                        if (afnd.Transiciones.TryGetValue(clave, out HashSet<string> alcanzables))
                        {
                            foreach (string dest in alcanzables)
                            {
                                conjuntoDestino.Add(dest);
                            }
                        }
                    }

                    // Manejo del Estado Trampa / Pozo
                    if (conjuntoDestino.Count == 0)
                    {
                        if (nombreEstadoTrampa == null)
                        {
                            nombreEstadoTrampa = "T";
                            afd.Estados.Add(nombreEstadoTrampa);
                            mapaEquivalencias[nombreEstadoTrampa] = new HashSet<string> { "Ø" };

                            foreach (char s in afnd.Alfabeto)
                            {
                                afd.AgregarTransicion(nombreEstadoTrampa, s, nombreEstadoTrampa);
                            }
                        }

                        afd.AgregarTransicion(nombreMacroActual, simbolo, nombreEstadoTrampa);
                        continue;
                    }

                    // Buscar si el conjunto obtenido ya fue registrado
                    string nombreMacroDestino = ObtenerNombreMacroEstado(conjuntoDestino, mapaEquivalencias);

                    // Si es un subconjunto completamente nuevo, se le asigna la letra actual ('A', 'B', 'C'...)
                    if (nombreMacroDestino == null)
                    {
                        nombreMacroDestino = letraActual.ToString();
                        letraActual++; // Avanza a la siguiente letra del alfabeto

                        mapaEquivalencias[nombreMacroDestino] = conjuntoDestino;
                        afd.Estados.Add(nombreMacroDestino);

                        colaPendientes.Enqueue(conjuntoDestino);
                    }

                    afd.AgregarTransicion(nombreMacroActual, simbolo, nombreMacroDestino);
                }
            }

            // 3. Identificar Estados Finales del AFD
            foreach (var par in mapaEquivalencias)
            {
                string nombreMacro = par.Key;
                HashSet<string> conjuntoAFND = par.Value;

                if (conjuntoAFND.Overlaps(afnd.EstadosFinales))
                {
                    afd.EstadosFinales.Add(nombreMacro);
                }
            }

            // 4. Desplegar la Tabla de Equivalencias
            MostrarTablaEquivalencias(mapaEquivalencias);

            return afd;
        }

        private static string ObtenerNombreMacroEstado(HashSet<string> conjuntoBuscado, Dictionary<string, HashSet<string>> mapa)
        {
            foreach (var par in mapa)
            {
                if (par.Value.SetEquals(conjuntoBuscado))
                {
                    return par.Key;
                }
            }
            return null;
        }

        private static void MostrarTablaEquivalencias(Dictionary<string, HashSet<string>> mapa)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("     TABLA DE EQUIVALENCIAS (SUB CONJUNTOS)   ");
            Console.WriteLine("==============================================");
            Console.WriteLine($"{"Macro-Estado (AFD)",-20} | {"Conjunto AFND Original",-25}");
            Console.WriteLine(new string('-', 50));

            foreach (var par in mapa)
            {
                string conjuntoStr = $"{{ {string.Join(", ", par.Value)} }}";
                Console.WriteLine($"{par.Key,-20} | {conjuntoStr,-25}");
            }
            Console.WriteLine(new string('-', 50) + "\n");
        }
    }
}