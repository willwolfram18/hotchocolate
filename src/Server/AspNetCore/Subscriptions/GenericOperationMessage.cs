using Newtonsoft.Json.Linq;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal class GenericOperationMessage
        : OperationMessage
    {
        public JObject Payload { get; set; }
    }
}
