namespace IssueButler.Chores
{
    using System;
    using System.Linq;
    using Octokit;

    class GetRepositoriesToWatch : Chore
    {
        readonly string organization;

        public GetRepositoriesToWatch(string organization)
        {
            this.organization = organization;
        }

        public override void PerformChore(Brain brain)
        {
            var client = brain.Recall<GitHubClient>();

            var repos = client.Repository.GetAllForOrg(organization).Result
                .Where(r => r.HasIssues && !r.Private && r.Name.StartsWith("NServiceBus") || r.Name.StartsWith("Service"))
                .Where(r=>r.Name != "NServiceBus.ActiveMQ") //until we can make the repo private or remove it
                .ToList();

            Console.Out.WriteLine("Asked to watch over org {0}", organization);

            brain.Remember(new RepositoriesToWatchOver(repos));

        }
    }
}