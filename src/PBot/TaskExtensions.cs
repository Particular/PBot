namespace PBot
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable IgnoreWaitContext(this Task t)
        {
            return t.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> IgnoreWaitContext<T>(this Task<T> t)
        {
            return t.ConfigureAwait(false);
        }
    }
}