namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;

    public class Issue_statistics //: BotCommandFixture<ListIssuesFromExternalContributors>
    {
        [Test, Explicit]
        public Task AllBugs()
        {
            Console.WriteLine("### All bugs");
            return GenerateReport();
        }

        public async Task GenerateReport()
        {
            var client = GitHubClientBuilder.Build();

            var period = DateTimeOffset.Parse("2015-01-01");

            var request = new RepositoryIssueRequest
            {
                Since = period,
                State = ItemState.All
            };

            var organization = "Particular";

            var members = await client.Organization.Member.GetAll(organization);

            var repos = await client.Repository.GetAllForOrg(organization);

            foreach (var repo in repos.Where(r => !r.Private))
            {
                var issues = await client.Issue.GetAllForRepository(organization, repo.Name, request);
                foreach (var issue in issues.Where(i => i.CreatedAt >= period))
                {
                    var createdByExternalUser = members.All(m => m.Login != issue.User.Login);
                    var isBug = issue.Labels.Any(l => l.Name.ToLower() == "bug");

                    if (isBug)
                    {
                        Console.WriteLine(string.Join(";", new List<string>
                        {
                            issue.HtmlUrl.ToString(),
                            issue.CreatedAt.ToString("d"),
                            createdByExternalUser.ToString(),
                            issue.User.Login
                        }));
                    }
                }
            }
        }
    }
}