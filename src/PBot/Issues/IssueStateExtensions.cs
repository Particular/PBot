namespace PBot
{
    using System.Linq;
    using System.Reflection;
    using Octokit;

    public static class IssueStateExtensions
    {
        public static bool IsInState(this Issue issue, IssueState state)
        {
            return issue.Labels.Any(l => l.Name == state);
        }

        public static bool IsInInitialState<T>(this Issue issue)
        {
            var allStates = typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(IssueState))
                .Select(f => (IssueState)f.GetValue(null));

            return !issue.Labels.Any(l => allStates.Any(s => s == l.Name));

        }

    }
}