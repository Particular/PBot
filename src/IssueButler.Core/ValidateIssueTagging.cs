namespace ReleaseStats.Providers.GitHub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class ValidateIssueTagging
    {
        readonly string organization;
        GitHubClient client;
        public ValidateIssueTagging(string organization)
        {
            this.organization = organization;
            client = GitHubClientBuilder.Build();
        }


        public IEnumerable<string> FindMatching(string filter)
        {
            var repos = client.Repository.GetAllForOrg(organization).Result;

            if (filter.EndsWith("*"))
            {
                var rootString = filter.Replace("*", "");
                return repos.Where(r => String.IsNullOrEmpty(rootString) || r.Name.StartsWith(rootString)).Select(p => p.Name).ToList();
            }
            
            throw new Exception("Invalid filter: " + filter + " only {name}* supported for now");
        }
    }
}