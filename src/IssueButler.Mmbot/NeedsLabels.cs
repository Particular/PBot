namespace IssueButler.Mmbot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class NeedsLabels
    {
        public static bool Contains(string name)
        {
            return labels.Any(l => String.Equals(l.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        static Label[] labels =
        {
            new Label
            {
                Name = "Needs: Scheduling",
                Color = "84b6eb"
            },
       new Label
            {
                Name = "Needs: Hotfix",
                Color = "84b6eb"
            },
       new Label
            {
                Name = "Needs: Triage",
                Color = "84b6eb"
            },
       new Label
            {
                Name = "Needs: Investigation",
                Color = "84b6eb"
            }
        };

        public static IEnumerable<Label> All
        {
            get { return labels; }
        }
    }
}