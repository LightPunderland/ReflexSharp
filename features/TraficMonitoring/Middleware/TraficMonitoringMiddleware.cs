using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Features.TraficMonitoring.Middleware
{
    public class TraficMonitoringMiddleware
    {
        private readonly RequestDelegate _next;

        public TraficMonitoringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = $"{context.Request.Method} {context.Request.Path}";
            Features.TraficMonitoring.TraficMonitoring.IncrementRequestCount(endpoint);
            await _next(context);
        }
    }
}