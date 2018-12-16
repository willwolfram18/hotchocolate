using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;

#if ASPNETCLASSIC
using Microsoft.Owin;
using HttpContext = Microsoft.Owin.IOwinContext;
#else
using Microsoft.AspNetCore.Http;
#endif

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal interface IWebSocketContext
        : IDisposable
    {
        HttpContext HttpContext { get; }

        IQueryExecuter QueryExecuter { get; }

        WebSocketCloseStatus? CloseStatus { get; }

        IDictionary<string, object> RequestProperties { get; }

        void RegisterSubscription(ISubscription subscription);

        void UnregisterSubscription(string subscriptionId);

        Task PrepareRequestAsync(QueryRequest request);

        Task SendMessageAsync(
            Stream messageStream,
            CancellationToken cancellationToken);

        Task ReceiveMessageAsync(
            Stream messageStream,
            CancellationToken cancellationToken);

        Task<ConnectionStatus> OpenAsync(
            IDictionary<string, object> properties);

        Task CloseAsync();
    }
}
