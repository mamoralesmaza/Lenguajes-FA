using System;
using System.Collections.Generic;

namespace SimuladorAFD
{
    /// Clase encargada de dar formato e imprimir la tabla de transición de un AFND.
    public static class TablaTransicionAFND
    {
        public static void MostrarTabla(AFND afnd)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("        TABLA DE TRANSICIÓN DEL AFND          ");
            Console.WriteLine("==============================================");

            // Imprimir la definición formal
            Console.WriteLine($"Q = {{ {string.Join(", ", afnd.Estados)} }}");
            Console.WriteLine($"Σ = {{ {string.Join(", ", afnd.Alfabeto)} }}");
            Console.WriteLine($"q0 = {afnd.EstadoInicial}");
            Console.WriteLine($"F = {{ {string.Join(", ", afnd.EstadosFinales)} }}\n");

            // Imprimir encabezado de la tabla
            Console.Write($"{"Estado",-12}");
            foreach (var simbolo in afnd.Alfabeto)
            {
                Console.Write($"|  {simbolo,-12}");
            }
            Console.WriteLine("\n" + new string('-', 12 + afnd.Alfabeto.Count * 15));

            // Imprimir cada estado de Q con sus transiciones
            foreach (var estado in afnd.Estados)
            {
                // Formatear prefijos -> (Inicial) y * (Final)
                string prefijo = "";
                if (estado == afnd.EstadoInicial) prefijo += "->";
                if (afnd.EstadosFinales.Contains(estado)) prefijo += "*";

                string estadoConPrefijo = string.IsNullOrEmpty(prefijo) ? estado : $"{prefijo}{estado}";
                Console.Write($"{estadoConPrefijo,-12}");

                foreach (var simbolo in afnd.Alfabeto)
                {
                    var clave = (estado, simbolo);
                    if (afnd.Transiciones.TryGetValue(clave, out HashSet<string> destinos) && destinos.Count > 0)
                    {
                        string conjuntoDestinos = $"{{{string.Join(",", destinos)}}}";
                        Console.Write($"|  {conjuntoDestinos,-12}");
                    }
                    else
                    {
                        // Si no hay transición definida para ese par
                        Console.Write($"|  {"Ø",-12}");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 12 + afnd.Alfabeto.Count * 15));
        }
    }
}