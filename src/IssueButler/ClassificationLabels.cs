namespace IssueButler
{
    using System.Collections.Generic;
    using System.Linq;

    public class ClassificationLabels
    {
        public static bool Contains(string name)
        {
            return labels.Contains(name);
        }

        static string[] labels =
        {
            "Bug",
            "Feature",
            "Improvement",
            "Internal Refactoring",
            "Question"
        };

        public static IEnumerable<string> All
        {
            get { return labels; }
        }
    }
}