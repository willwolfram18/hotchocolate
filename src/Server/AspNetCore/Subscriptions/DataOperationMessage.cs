using System.Collections.Generic;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal class DataOperationMessage
        : OperationMessage
    {
        public IReadOnlyDictionary<string, object> Payload { get; set; }
    }
}
