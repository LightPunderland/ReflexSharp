using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Features.TraficMonitoring
{
    public class TraficMonitoring
    {
        private static readonly ConcurrentDictionary<string, int> RequestCounts = new();
        private static readonly object ConsoleLock = new();

        public static void IncrementRequestCount(string endpoint)
        {
            RequestCounts.AddOrUpdate(endpoint, 1, (_, count) => count + 1);
            DisplayCurrentStats();
        }

        private static void DisplayCurrentStats()
        {
            lock (ConsoleLock)
            {
                Console.Clear();
                Console.WriteLine("=== API Request Monitor ===");
                Console.WriteLine($"Time: {DateTime.Now}");
                Console.WriteLine("\nEndpoint Requests:");
                foreach (var (endpoint, count) in RequestCounts)
                {
                    Console.WriteLine($"{endpoint}: {count} requests");
                }
            }
        }
    }
}