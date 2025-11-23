using System.Diagnostics;

namespace CareerUp.Observability
{
    /// <summary>
    /// Classe para criação de spans customizados de tracing
    /// </summary>
    public static class Tracing
    {
        private static readonly ActivitySource ActivitySource = new("CareerUp.Api");

        /// <summary>
        /// Cria um novo span de atividade para tracing
        /// </summary>
        /// <param name="operationName">Nome da operação</param>
        /// <param name="kind">Tipo da atividade</param>
        /// <returns>Activity ou null</returns>
        public static Activity? StartActivity(string operationName, ActivityKind kind = ActivityKind.Internal)
        {
            return ActivitySource.StartActivity(operationName, kind);
        }

        /// <summary>
        /// Adiciona tag a uma atividade
        /// </summary>
        public static void AddTag(this Activity? activity, string key, object? value)
        {
            activity?.SetTag(key, value);
        }

        /// <summary>
        /// Adiciona evento a uma atividade
        /// </summary>
        public static void AddEvent(this Activity? activity, string eventName)
        {
            activity?.AddEvent(new ActivityEvent(eventName));
        }

        /// <summary>
        /// Retorna o ActivitySource para configuração no Program.cs
        /// </summary>
        public static ActivitySource GetActivitySource() => ActivitySource;
    }
}
