namespace IssueButler
{
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    class Butler
    {
        protected Butler(string organization)
        {
            this.organization = organization;
            client = GitHubClientBuilder.Build();
        }

        public void PerformChores()
        {
            var repos = client.Repository.GetAllForOrg(organization).Result
             .Where(r => r.HasIssues && !r.Private && r.Name.StartsWith("NServiceBus") || r.Name.StartsWith("Service"))
             .ToList();

            var validationErrors = Validators.SelectMany(v => v.Validate(repos)).ToList();

            Displayers.ForEach(d => d.Display(validationErrors));
        }

        protected List<Validator> Validators = new List<Validator>();

        protected List<ResultDisplayer> Displayers = new List<ResultDisplayer>();

        readonly string organization;
        GitHubClient client;
    }
}