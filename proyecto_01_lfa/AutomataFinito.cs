using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// Representa formalmente un Autómata Finito Determinista (AFD) mediante su túpla M = (Q, Σ, δ, q0, F).
    public class AutomataFinito
    {
        // Q: Conjunto finito de estados
        public HashSet<string> Estados { get; set; }

        // Σ (Sigma) / A: Alfabeto de entrada (conjunto finito de símbolos)
        public HashSet<char> Alfabeto { get; set; }

        // S / q0: Estado inicial
        public string EstadoInicial { get; set; }

        // F: Conjunto de estados finales o de aceptación (F ⊆ Q)
        public HashSet<string> EstadosFinales { get; set; }

        // δ (Delta) / T: Función de transición mapping (Estado, Símbolo) -> Estado Siguiente
        public Dictionary<(string Estado, char Simbolo), string> Transiciones { get; set; }

        /// Constructor que inicializa las estructuras de datos requeridas para el AFD.
        public AutomataFinito()
        {
            Estados = new HashSet<string>();
            Alfabeto = new HashSet<char>();
            EstadosFinales = new HashSet<string>();
            Transiciones = new Dictionary<(string Estado, char Simbolo), string>();
        }

        public void AgregarTransicion(string origen, char simbolo, string destino)
        {
            // Registramos la transición mapeando la tupla de entrada hacia el estado de llegada
            Transiciones[(origen, simbolo)] = destino;
        }
    }
}