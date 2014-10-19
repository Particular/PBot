namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class ResolutionLabels
    {
        public static bool Contains(string name)
        {
            return labels.Any(l => String.Equals(l.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        static Label[] labels =
        {
            new Label
            {
                Name = "Resolution: Won't Fix",
                Color = "444444"
            },
                new Label
            {
                Name = "Resolution: Can't Reproduce",
                Color = "444444"
            },
                new Label
            {
                Name = "Resolution: Duplicate",
                Color = "444444"
            }
        };

        public static IEnumerable<Label> All
        {
            get { return labels; }
        }
    }
}