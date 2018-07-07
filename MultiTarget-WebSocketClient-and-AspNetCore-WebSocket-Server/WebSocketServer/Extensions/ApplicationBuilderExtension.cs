using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebSocketServer.Handlers;
using WebSocketServer.Managers;
using WebSocketServer.Middlewares;

namespace WebSocketServer.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                    PathString path,
                                                    WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
    }
}
