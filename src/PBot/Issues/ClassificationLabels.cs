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
            return labels.Any(l => String.Equals(l.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        static Label[] labels =
        {
            new Label(null, "Bug", "fc2929"),
            new Label(null, "Feature", "159818"),
            new Label(null, "Improvement", "159818"),
            new Label(null, "Internal Refactoring","159818"),
            new Label(null, "Question", "84b6eb")
        };

    

        public static IEnumerable<Label> All
        {
            get { return labels; }
        }
    }
}