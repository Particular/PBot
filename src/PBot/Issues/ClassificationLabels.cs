namespace PBot.Issues
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class ClassificationLabels
    {
        public static bool Contains(string name)
        {
            return labels.Any(l => string.Equals(l.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        static Label[] labels =
        {
            new Label(null, "Type: Bug", "fc2929"),
            new Label(null, "Type: Feature", "159818"),
            new Label(null, RefactoringLabelName,"159818"),
            new Label(null, "Type: Question", "84b6eb")
        };

        public static string RefactoringLabelName => "Type: Refactoring";

        public static IEnumerable<Label> All => labels;
    }
}