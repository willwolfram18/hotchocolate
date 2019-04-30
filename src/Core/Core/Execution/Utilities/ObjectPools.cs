namespace HotChocolate.Execution
{
    internal static class ObjectPools
    {
        public static ObjectPool<ResolverTask> ResolverTasks { get; } =
            new ObjectPool<ResolverTask>(500);
    }
}
