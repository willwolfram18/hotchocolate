using System.Collections.Generic;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal class DictionaryOperationMessage
        : OperationMessage
    {
        public IReadOnlyDictionary<string, object> Payload { get; set; }
    }
}
