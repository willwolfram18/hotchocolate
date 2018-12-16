using System.Threading;
using System.Threading.Tasks;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal sealed class ConnectionTerminateHandler
        : IRequestHandler
    {
        public bool CanHandle(GenericOperationMessage message)
        {
            return message.Type == MessageTypes.Connection.Terminate;
        }

        public async Task HandleAsync(
            IWebSocketContext context,
            GenericOperationMessage message,
            CancellationToken cancellationToken)
        {
            await context.CloseAsync();
        }
    }
}
