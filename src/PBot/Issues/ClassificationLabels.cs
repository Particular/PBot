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
            new Label
            {
                Name = "Bug",
                Color = "fc2929"
            },
                new Label
            {
                Name = "Feature",
                Color = "159818"
            },
                new Label
            {
                Name = "Improvement",
                Color = "159818"
            },
                new Label
            {
                Name = "Internal Refactoring",
                Color = "159818"
            },
                new Label
            {
                Name = "Question",
                Color = "84b6eb"
            }
     
        };

        public static IEnumerable<Label> All
        {
            get { return labels; }
        }
    }
}