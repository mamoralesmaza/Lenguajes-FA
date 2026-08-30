using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// <summary>
    /// Clase encargada de verificar el cumplimiento de las restricciones teóricas
    /// del Autómata Finito Determinista (AFD) según su quintúpla.
    /// </summary>
    public class ValidadorAFD
    {
        /// <summary>
        /// Realiza las tres validaciones teóricas de integridad sobre la quintúpla del AFD.
        /// </summary>
        /// <param name="afd">Objeto del autómata a validar</param>
        /// <param name="errores">Lista donde se registrarán los errores encontrados</param>
        /// <returns>True si es un AFD válido e íntegro, False de lo contrario</returns>
        public static bool ValidarIntegridad(AutomataFinito afd, out List<string> errores)
        {
            errores = new List<string>();

            // ==========================================
            // 1. VALIDACIÓN DE ESTADOS
            // ==========================================
            // Verificar que el estado inicial pertenezca a Q (q0 ∈ Q)
            if (string.IsNullOrEmpty(afd.EstadoInicial) || !afd.Estados.Contains(afd.EstadoInicial))
            {
                errores.Add($"[Error de Estado] El estado inicial '{afd.EstadoInicial}' no pertenece al conjunto de estados Q.");
            }

            // Verificar que los estados finales sean un subconjunto de Q (F ⊆ Q)
            foreach (var estadoFinal in afd.EstadosFinales)
            {
                if (!afd.Estados.Contains(estadoFinal))
                {
                    errores.Add($"[Error de Estado] El estado final '{estadoFinal}' no pertenece al conjunto de estados Q.");
                }
            }

            // ==========================================
            // 2. CONSISTENCIA DE TRANSICIONES
            // ==========================================
            // Verificar que los estados origen, destino y símbolos usados en δ existan en Q y Σ
            foreach (var transicion in afd.Transiciones)
            {
                string origen = transicion.Key.Estado;
                char simbolo = transicion.Key.Simbolo;
                string destino = transicion.Value;

                if (!afd.Estados.Contains(origen))
                {
                    errores.Add($"[Error de Transición] El estado origen '{origen}' en la transición no pertenece a Q.");
                }

                if (!afd.Alfabeto.Contains(simbolo))
                {
                    errores.Add($"[Error de Transición] El símbolo '{simbolo}' en la transición no pertenece al alfabeto A.");
                }

                if (!afd.Estados.Contains(destino))
                {
                    errores.Add($"[Error de Transición] El estado destino '{destino}' en la transición no pertenece a Q.");
                }
            }

            // ==========================================
            // 3. VERIFICACIÓN DETERMINISTA
            // ==========================================
            // Para cada par (q ∈ Q, a ∈ Σ) debe existir EXACTAMENTE una transición.
            foreach (var estado in afd.Estados)
            {
                foreach (var simbolo in afd.Alfabeto)
                {
                    // Si no existe una definición para el par (estado, simbolo), falta una transición
                    if (!afd.Transiciones.ContainsKey((estado, simbolo)))
                    {
                        errores.Add($"[Error Determinista] Falta la transición para el par ({estado}, '{simbolo}'). " +
                                    "El autómata no es totalmente determinado o es un AFND.");
                    }
                }
            }

            // Si no hay errores, la integridad es válida
            return errores.Count == 0;
        }
    }
}