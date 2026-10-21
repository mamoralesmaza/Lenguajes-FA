using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// Representa la séptupla de un Autómata de Pila Determinista: M = (Q, Σ, Γ, q0, Z0, F, δ)
    public class APD
    {
        /// Q: Conjunto de estados del APD
        public HashSet<string> Estados { get; set; }

        /// Σ (A): Alfabeto de entrada
        public HashSet<char> AlfabetoEntrada { get; set; }

        /// Γ (R): Alfabeto de la Pila
        public HashSet<char> AlfabetoPila { get; set; }

        /// S (q0): Estado inicial
        public string EstadoInicial { get; set; } = string.Empty;

        /// F: Conjunto de estados finales / de aceptación
        public HashSet<string> EstadosFinales { get; set; }

        /// Z0: Símbolo inicial de la pila
        public char SimboloInicialPila { get; set; }

        /// Lista de transiciones delta del APD
        public List<TransicionAPD> Transiciones { get; set; }

        /// Constructor predeterminado que inicializa los conjuntos de la séptupla.
        public APD()
        {
            Estados = new HashSet<string>();
            AlfabetoEntrada = new HashSet<char>();
            AlfabetoPila = new HashSet<char>();
            EstadosFinales = new HashSet<string>();
            Transiciones = new List<TransicionAPD>();
        }

        /// Agrega una transición a la lista del APD.
        public void AgregarTransicion(string origen, char entrada, char tope, string destino, string apilar)
        {
            Transiciones.Add(new TransicionAPD(origen, entrada, tope, destino, apilar));
        }

        /// Valida la condición estricta de determinismo requerida para el APD.
        /// Retorna (true, "") si es válido, o (false, "motivo") notificando el conflicto detectado.
        public (bool EsValido, string MensajeError) ValidarDeterminismo()
        {
            // Mapeo para verificar duplicados directos: (Estado, Entrada, TopePila)
            HashSet<(string, char, char)> reglasExistentes = new HashSet<(string, char, char)>();

            foreach (var t in Transiciones)
            {
                var clave = (t.EstadoOrigen, t.SimboloEntrada, t.TopePila);

                // 1. Verificación de duplicación directa: dos transiciones definidas para la misma terna (q, a, Z)
                if (reglasExistentes.Contains(clave))
                {
                    return (false, $"Conflicto de determinismo: Existen múltiples transiciones para la terna ({t.EstadoOrigen}, '{t.SimboloEntrada}', '{t.TopePila}').");
                }
                reglasExistentes.Add(clave);
            }

            // 2. Verificación del conflicto con ε (transición nula):
            // Si existe δ(q, 'e', Z), NO puede existir δ(q, a, Z) para ningún a en el alfabeto.
            foreach (var tEpsilon in Transiciones)
            {
                // Identificamos 'e' o 'ε' como símbolo nulo
                if (tEpsilon.SimboloEntrada == 'e' || tEpsilon.SimboloEntrada == 'ε')
                {
                    foreach (var tOtra in Transiciones)
                    {
                        // Misma regla de estado y mismo tope de pila, pero con un símbolo de entrada real diferente de 'e'/'ε'
                        if (tOtra.EstadoOrigen == tEpsilon.EstadoOrigen &&
                            tOtra.TopePila == tEpsilon.TopePila &&
                            tOtra.SimboloEntrada != 'e' && tOtra.SimboloEntrada != 'ε')
                        {
                            return (false, $"Conflicto de determinismo con ε: Para el estado '{tEpsilon.EstadoOrigen}' con tope de pila '{tEpsilon.TopePila}', existe la transición épsilon δ({tEpsilon.EstadoOrigen}, ε, {tEpsilon.TopePila}) y también δ({tOtra.EstadoOrigen}, '{tOtra.SimboloEntrada}', {tOtra.TopePila}).");
                        }
                    }
                }
            }

            return (true, "El autómata cumple estrictamente la condición de APD Determinista.");
        }
    }
}