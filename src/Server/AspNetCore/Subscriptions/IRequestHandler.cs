using System.Threading;
using System.Threading.Tasks;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal interface IRequestHandler
    {
        Task HandleAsync(
            IWebSocketContext context,
            GenericOperationMessage message,
            CancellationToken cancellationToken);

        bool CanHandle(GenericOperationMessage message);
    }
}
