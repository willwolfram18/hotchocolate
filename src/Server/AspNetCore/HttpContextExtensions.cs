using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

#if ASPNETCLASSIC
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
using WebSocketAccept = System.Action<System.Collections.Generic.IDictionary<string, object>,
    System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>;
#else
using Microsoft.AspNetCore.Http;
#endif

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic
#else
namespace HotChocolate.AspNetCore
#endif
{
    internal static class HttpContextExtensions
    {
#if ASPNETCLASSIC
        private const string _webSocketAccept = "websocket.Accept";
#endif

        public static Task<WebSocket> AcceptWebSocketAsync(
            this HttpContext context,
            string subProtocol)
        {
#if ASPNETCLASSIC
            var acceptDictionary = new Dictionary<string, object>
            {
                { "websocket.SubProtocol", subProtocol }
            };
            WebSocketAccept accept = context.Get<WebSocketAccept>(
                _webSocketAccept);

            accept(acceptDictionary, )

                WebSocket.
#else
            return context.WebSockets.AcceptWebSocketAsync(subProtocol);
#endif
        }

#if ASPNETCLASSIC
        public static IServiceProvider CreateRequestServices(
            this HttpContext context,
            IServiceProvider rootServiceProvider)
        {
            var services = new Dictionary<Type, object>
            {
                { typeof(HttpContext), context }
            };
            var serviceProvider = new RequestServiceProvider(
                rootServiceProvider,
                services);

            context.Environment.Add(EnvironmentKeys.ServiceProvider,
                serviceProvider);

            return serviceProvider;
        }
#else

        public static IServiceProvider CreateRequestServices(
            this HttpContext context)
        {
            var services = new Dictionary<Type, object>
            {
                { typeof(HttpContext), context }
            };

            return new RequestServiceProvider(
                context.RequestServices, services);
        }
#endif

        public static CancellationToken GetCancellationToken(
            this HttpContext context)
        {
#if ASPNETCLASSIC
            return context.Request.CallCancelled;
#else
            return context.RequestAborted;
#endif
        }

        public static IPrincipal GetUser(this HttpContext context)
        {
#if ASPNETCLASSIC
            return context.Request.User;
#else
            return context.User;
#endif
        }

        public static bool IsValidPath(
            this HttpContext context,
            PathString path)
        {
            return context.Request.Path.StartsWithSegments(path);
        }

        public static bool IsWebSocketRequest(
            this HttpContext context)
        {
#if ASPNETCLASSIC
            return context.Get<WebSocketAccept>("websocket.Accept") != null;
#else
            return context.WebSockets.IsWebSocketRequest;
#endif
        }
    }
}
