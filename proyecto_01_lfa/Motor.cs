using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// Funcionalidad 2: Motor de Validación de la Quintúpla
    /// Funcionalidad 2: Motor de Validación de la Quintúpla.
    /// Valida la integridad estructural e invariantes teóricas del AFD antes de procesar cadenas.
    public class Motor
    {
        /// <summary>
        /// Comprueba las 3 reglas teóricas: q0 in Q y F subseteq Q, consistencia de T en (Q, Sigma),
        /// y comportamiento estrictamente determinista.
        /// </summary>
        /// <param name="afd">Instancia del autómata a validar</param>
        /// <param name="errores">Lista con la descripción detallada de fallos detectados</param>
        /// <returns>True si el autómata es un AFD válido, False de lo contrario</returns>
        public static bool ValidarIntegridad(AutomataFinito afd, out List<string> errores)
        {
            errores = new List<string>();

            // 1. VALIDACIÓN DE ESTADOS: q0 ∈ Q y F ⊆ Q
            if (string.IsNullOrEmpty(afd.EstadoInicial) || !afd.Estados.Contains(afd.EstadoInicial))
            {
                errores.Add($"[Error de Estado] El estado inicial '{afd.EstadoInicial}' no pertenece al conjunto Q.");
            }

            foreach (var estadoFinal in afd.EstadosFinales)
            {
                if (!afd.Estados.Contains(estadoFinal))
                {
                    errores.Add($"[Error de Estado] El estado final '{estadoFinal}' no pertenece al conjunto Q.");
                }
            }

            // 2. CONSISTENCIA DE TRANSICIONES: Todos los elementos de delta deben pertenecer a Q y A (Σ)
            foreach (var transicion in afd.Transiciones)
            {
                string origen = transicion.Key.Estado;
                char simbolo = transicion.Key.Simbolo;
                string destino = transicion.Value;

                if (!afd.Estados.Contains(origen))
                    errores.Add($"[Error de Transición] Estado origen '{origen}' no pertenece a Q.");

                if (!afd.Alfabeto.Contains(simbolo))
                    errores.Add($"[Error de Transición] Símbolo '{simbolo}' no pertenece al alfabeto A.");

                if (!afd.Estados.Contains(destino))
                    errores.Add($"[Error de Transición] Estado destino '{destino}' no pertenece a Q.");
            }

            // 3. VERIFICACIÓN DETERMINISTA: Exactamente una transición para cada par (estado, símbolo)
            foreach (var estado in afd.Estados)
            {
                foreach (var simbolo in afd.Alfabeto)
                {
                    if (!afd.Transiciones.ContainsKey((estado, simbolo)))
                    {
                        errores.Add($"[Error Determinista] Falta la transición para el par ({estado}, '{simbolo}'). " +
                                    "La entrada corresponde a un AFND o está incompleta.");
                    }
                }
            }

            return errores.Count == 0;
        }
    }
}