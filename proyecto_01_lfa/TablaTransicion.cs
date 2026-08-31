namespace SimuladorAFD
{
    /// <summary>
    /// Funcionalidad 3: Generación y Despliegue de la Tabla de Transición.
    /// Se encarga únicamente del despliegue visual formal y matricial del AFD.
    /// </summary>
    public class TablaTransicion
    {
        /// Funcionalidad 3: Generación y despliegue de la Tabla de Transición
        /// Imprime en consola la quintúpla formal y genera la matriz visual de transiciones.
        /// </summary>
        /// <param name="afd">Autómata del cual se desplegarán los datos</param>
        public static void MostrarTablaTransicion(AutomataFinito afd)
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("        DEFINICIÓN FORMAL DEL AFD        ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Q = {{ {string.Join(", ", afd.Estados)} }}");
            Console.WriteLine($"A = {{ {string.Join(", ", afd.Alfabeto)} }}");
            Console.WriteLine($"S = {afd.EstadoInicial}");
            Console.WriteLine($"F = {{ {string.Join(", ", afd.EstadosFinales)} }}");

            Console.WriteLine("\n==========================================");
            Console.WriteLine("           TABLA DE TRANSICIÓN            ");
            Console.WriteLine("==========================================");

            // Imprimir encabezado de las columnas (Alfabeto)
            Console.Write($"{"Estado",-12}");
            foreach (var simbolo in afd.Alfabeto)
            {
                Console.Write($"|  {simbolo,-6}");
            }
            Console.WriteLine("\n" + new string('-', 12 + afd.Alfabeto.Count * 9));

            // Imprimir filas por cada estado de Q
            foreach (var estado in afd.Estados)
            {
                string prefijo = "";
                if (estado == afd.EstadoInicial) prefijo += "->";
                if (afd.EstadosFinales.Contains(estado)) prefijo += "*";

                string estadoFormateado = $"{prefijo}{estado}";
                Console.Write($"{estadoFormateado,-12}");

                foreach (var simbolo in afd.Alfabeto)
                {
                    if (afd.Transiciones.TryGetValue((estado, simbolo), out string destino))
                    {
                        Console.Write($"|  {destino,-6}");
                    }
                    else
                    {
                        Console.Write($"|  {"-",-6}");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("==========================================\n");
        }
    }
}