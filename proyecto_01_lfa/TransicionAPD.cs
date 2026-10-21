using System;

namespace SimuladorAFD
{
    /// Representa una transición individual en un Autómata de Pila Determinista (APD).
    public class TransicionAPD
    {
        /// Estado de origen (q)
        public string EstadoOrigen { get; set; }

        /// Símbolo leído de la cadena de entrada ('a' o 'e' / 'ε' para transición nula)
        public char SimboloEntrada { get; set; }

        /// Símbolo desapilado del tope de la pila (Z)
        public char TopePila { get; set; }

        /// Estado de destino al que se transiciona (p)
        public string EstadoDestino { get; set; }

        /// Cadena de símbolos que se apilan en orden (γ).
        /// Nota: Si es "e" o "ε", representa palabra vacía (no apila nada / desapilado efectivo).
        public string CadenaApilar { get; set; }

        /// Constructor principal para inicializar todos los campos de una transición de APD.
        public TransicionAPD(string origen, char entrada, char tope, string destino, string apilar)
        {
            EstadoOrigen = origen;
            SimboloEntrada = entrada;
            TopePila = tope;
            EstadoDestino = destino;
            CadenaApilar = apilar;
        }
    }
}