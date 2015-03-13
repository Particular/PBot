namespace PBot.Buildserver
{
    using System.Collections.Generic;

    public class ProjectDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public List<BuildType> BuildTypes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}