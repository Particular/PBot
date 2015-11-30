namespace PBot
{
    using System;
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


        public static IssueState CurrentState<T>(this Issue issue)
        {
            if (issue.IsInInitialState<T>())
            {
                return "State: Initial";
            }

            var allStates = typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(IssueState))
                .Select(f => (IssueState)f.GetValue(null))
                .ToList();

            var actualStates = issue.Labels.Where(l => allStates.Contains(l.Name))
                .Select(l=>l.Name)
                .ToList();

            if (actualStates.Any())
            {
                if (actualStates.Count > 1)
                {
                    throw new Exception($"Issue {issue.HtmlUrl} has multiple states - {string.Join(";", actualStates)}");
                }

                return actualStates.Single();
            }
            else
            {
                return "State: Initial";         
            }
        }
    }
}