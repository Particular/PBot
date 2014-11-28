namespace IssueButler.Mmbot
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable IgnoreWaitContext(this Task t)
        {
            return t.ConfigureAwait(false);
        }
    }
}