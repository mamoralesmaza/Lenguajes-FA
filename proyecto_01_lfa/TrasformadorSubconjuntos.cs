using System;
using System.Collections.Generic;
using System.Linq;

namespace SimuladorAFD
{
    /// Clase estática encargada de aplicar el Algoritmo de Construcción de Subconjuntos (Power Set Construction).
    /// Transforma un Autómata Finito No Determinista (AFND) en un Autómata Finito Determinista (AFD) equivalente.
    /// Preserva la etiqueta del estado inicial original (ej. "q0") y asigna identificadores secuenciales 
    /// ('A', 'B', 'C'...) a los macro-estados subsiguientes.
    public static class TransformadorSubconjuntos
    {
        /// Procesa la conversión de un AFND a un AFD mediante la evaluación iterativa de conjuntos de estados alcanzables.
        /// <param name="afnd">Instancia del AFND de origen a transformar.</param>
        public static AutomataFinito ConvertirAFNDaAFD(AFND afnd)
        {
            // Instancia del nuevo autómata determinista resultante
            AutomataFinito afd = new AutomataFinito();

            // Copia exacta del alfabeto Σ del AFND al nuevo AFD
            afd.Alfabeto = new HashSet<char>(afnd.Alfabeto);

            // Mapeo de equivalencia: Asocia el nombre de un macro-estado del AFD (ej. "A") 
            // con su conjunto correspondiente de estados del AFND original (ej. {"q0", "q1"})
            Dictionary<string, HashSet<string>> mapaEquivalencias = new Dictionary<string, HashSet<string>>();

            // Cola de procesamiento (BFS) para evaluar macro-estados descubiertos pendientes de exploración
            Queue<HashSet<string>> colaPendientes = new Queue<HashSet<string>>();

            // Contador de letras para nombrar secuencialmente los nuevos macro-estados generados
            char letraActual = 'A';

            // Identificador para el estado trampa/pozo (permanece nulo si el autómata no lo requiere)
            string nombreEstadoTrampa = null;

            // PASO 1: INICIALIZACIÓN DEL ESTADO INICIAL
            // El conjunto inicial del AFD contiene únicamente al estado inicial del AFND
            HashSet<string> conjuntoInicial = new HashSet<string> { afnd.EstadoInicial };
            string nombreInicial = afnd.EstadoInicial; // Preserva la nomenclatura (ej. "q0")

            // Registrar el estado inicial en las estructuras del AFD
            mapaEquivalencias[nombreInicial] = conjuntoInicial;
            afd.Estados.Add(nombreInicial);
            afd.EstadoInicial = nombreInicial;

            // Encolar el estado inicial para explorar sus salidas hacia el alfabeto
            colaPendientes.Enqueue(conjuntoInicial);

            // PASO 2: BUCLE PRINCIPAL DE CONSTRUCCIÓN DE SUBCONJUNTOS
            while (colaPendientes.Count > 0)
            {
                // Extraer el conjunto de estados actual a evaluar
                HashSet<string> conjuntoActual = colaPendientes.Dequeue();

                // Obtener el nombre asignado en el AFD para este conjunto de estados
                string nombreMacroActual = ObtenerNombreMacroEstado(conjuntoActual, mapaEquivalencias);

                // Evaluar la respuesta del conjunto frente a cada símbolo del alfabeto 
                foreach (char simbolo in afnd.Alfabeto)
                {
                    // Almacena la unión de todos los estados alcanzables leyendo 'simbolo'
                    HashSet<string> conjuntoDestino = new HashSet<string>();

                    // Calcular la unión de salidas (q, simbolo) para cada estado q en conjuntoActual
                    foreach (string estadoAFND in conjuntoActual)
                    {
                        var clave = (estadoAFND, simbolo);

                        // Si existe una regla de transición registrada para el par (estado, simbolo)
                        if (afnd.Transiciones.TryGetValue(clave, out HashSet<string> alcanzables))
                        {
                            foreach (string dest in alcanzables)
                            {
                                conjuntoDestino.Add(dest); // HashSet evita duplicados automáticamente
                            }
                        }
                    }

                    // MANEJO DE TRANSICIONES A CONJUNTO VACÍO (ESTADO TRAMPA)
                    if (conjuntoDestino.Count == 0)
                    {
                        // Si es la primera vez que se detecta una transición vacía, inicializar el estado TRAMPA
                        if (nombreEstadoTrampa == null)
                        {
                            nombreEstadoTrampa = "T"; // Asignación del nombre del estado trampa
                            afd.Estados.Add(nombreEstadoTrampa);
                            mapaEquivalencias[nombreEstadoTrampa] = new HashSet<string> { "Ø" };

                            // Definir autobucles en el estado trampa para todos los símbolos del alfabeto 
                            foreach (char s in afnd.Alfabeto)
                            {
                                afd.AgregarTransicion(nombreEstadoTrampa, s, nombreEstadoTrampa);
                            }
                        }

                        // Conectar la transición sin salida hacia el estado trampa
                        afd.AgregarTransicion(nombreMacroActual, simbolo, nombreEstadoTrampa);
                        continue; // Continuar con el siguiente símbolo del alfabeto
                    }

                    // PROCESAMIENTO DE CONJUNTO DESTINO NO VACÍO
                    // Verificar si este conjunto de estados ya ha sido registrado previamente
                    string nombreMacroDestino = ObtenerNombreMacroEstado(conjuntoDestino, mapaEquivalencias);

                    // Si el conjunto evaluado es completamente nuevo en la estructura
                    if (nombreMacroDestino == null)
                    {
                        // Asignar la siguiente letra disponible ('A', 'B', 'C'...)
                        nombreMacroDestino = letraActual.ToString();
                        letraActual++; // Avanzar el puntero al siguiente carácter ASCII

                        // Registrar el nuevo macro-estado en las estructuras del AFD
                        mapaEquivalencias[nombreMacroDestino] = conjuntoDestino;
                        afd.Estados.Add(nombreMacroDestino);

                        // Encolar el nuevo conjunto para explorar sus salidas posteriormente
                        colaPendientes.Enqueue(conjuntoDestino);
                    }

                    // Registrar la transición determinista δ(MacroActual, simbolo) -> MacroDestino
                    afd.AgregarTransicion(nombreMacroActual, simbolo, nombreMacroDestino);
                }
            }

            // PASO 3: IDENTIFICACIÓN DE ESTADOS FINALES / ACEPTACIÓN
            // Un macroestado del AFD es final si contiene AL MENOS UN estado final del AFND original
            foreach (var par in mapaEquivalencias)
            {
                string nombreMacro = par.Key;
                HashSet<string> conjuntoAFND = par.Value;

                // Determinar si existe intersección entre el conjunto del macro-estado y F (AFND)
                if (conjuntoAFND.Overlaps(afnd.EstadosFinales))
                {
                    afd.EstadosFinales.Add(nombreMacro);
                }
            }

            // PASO 4: DESPLIEGUE DE RESULTADOS
            // Imprimir la tabla que relaciona las etiquetas del AFD con sus subconjuntos del AFND
            MostrarTablaEquivalencias(mapaEquivalencias);

            return afd;
        }

        /// Compara un conjunto de estados buscado contra la tabla de equivalencias registrada.
        /// <param name="conjuntoBuscado">Conjunto de estados del AFND a verificar.</param>
        /// <param name="mapa">Diccionario de mapeo de equivalencias actual.</param>
        /// <returns>El nombre del macroestado si ya existe; de lo contrario, null.</returns>
        private static string ObtenerNombreMacroEstado(HashSet<string> conjuntoBuscado, Dictionary<string, HashSet<string>> mapa)
        {
            foreach (var par in mapa)
            {
                // SetEquals verifica la igualdad exacta de contenido de dos conjuntos sin importar el orden
                if (par.Value.SetEquals(conjuntoBuscado))
                {
                    return par.Key;
                }
            }
            return null;
        }

        /// Formatea e imprime en la consola la tabla de correspondencia entre macro-estados y conjuntos del AFND.
        /// <param name="mapa">Diccionario que contiene el registro de equivalencias.</param>
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