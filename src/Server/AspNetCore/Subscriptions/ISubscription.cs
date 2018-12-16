using System;

#if ASPNETCLASSIC
namespace HotChocolate.AspNetClassic.Subscriptions
#else
namespace HotChocolate.AspNetCore.Subscriptions
#endif
{
    internal interface ISubscription
        : IDisposable
    {
        event EventHandler Completed;

        string Id { get; }
    }
}
