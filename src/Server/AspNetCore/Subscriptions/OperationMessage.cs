#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal class OperationMessage
    {
        public string Id { get; set; }

        public string Type { get; set; }
    }
}
