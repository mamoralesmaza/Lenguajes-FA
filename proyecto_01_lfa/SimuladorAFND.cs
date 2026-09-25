using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// Representa la quintúpla M = (Q, Σ, δ, q0, F) de un Autómata Finito No Determinista (AFND).
    public class AFND
    {
        // Q: Conjunto de estados del AFND
        public HashSet<string> Estados { get; set; }

        // Σ (A): Alfabeto de entrada
        public HashSet<char> Alfabeto { get; set; }

        // S (q0): Estado inicial
        public string EstadoInicial { get; set; }

        // F: Conjunto de estados finales o de aceptación
        public HashSet<string> EstadosFinales { get; set; }

        // δ (T): Función de transición. 
        // Mapea la pareja (EstadoOrigen, Símbolo) -> Conjunto de Estados Destino (HashSet<string>)
        public Dictionary<(string Estado, char Simbolo), HashSet<string>> Transiciones { get; set; }

        /// Constructor que inicializa los conjuntos vacíos para evitar referencias nulas.
        public AFND()
        {
            Estados = new HashSet<string>();
            Alfabeto = new HashSet<char>();
            EstadosFinales = new HashSet<string>();
            Transiciones = new Dictionary<(string, char), HashSet<string>>();
        }

        /// Agrega una transición al AFND. Si el par (origen, simbolo) ya existe, 
        /// añade el nuevo destino al conjunto de estados de llegada.
        /// </summary>
        /// <param name="origen">Estado de partida</param>
        /// <param name="simbolo">Símbolo del alfabeto leído</param>
        /// <param name="destino">Nuevo estado alcanzable</param>
        public void AgregarTransicion(string origen, char simbolo, string destino)
        {
            var clave = (origen, simbolo);

            // Si es la primera vez que se define una transición para este par, inicializamos su conjunto
            if (!Transiciones.ContainsKey(clave))
            {
                Transiciones[clave] = new HashSet<string>();
            }

            // Agregamos el estado destino al conjunto (HashSet evita duplicados automáticamente)
            Transiciones[clave].Add(destino);
        }
    }
}