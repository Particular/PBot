namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Octokit;

    public class Bug_statistics
    {
        [Test, Explicit]
        public async void GenerateReport()
        {
            var client = GitHubClientBuilder.Build();

            var request = new RepositoryIssueRequest();

          //  request.Labels.Add("Bug");
            request.Since = DateTimeOffset.Parse("2014-01-01");


            var organisation = "Particular";

            var members = await client.Organization.Member.GetAll(organisation);

            var repos = await client.Repository.GetAllForOrg(organisation);

            foreach (var repo in repos.Where(r=>!r.Private))
            {
                var bugs = await client.Issue.GetForRepository(organisation, repo.Name, request);
                foreach (var bug in bugs)
                {
                    if (members.All(m=>m.Login != bug.User.Login))
                    {
                          Console.Out.WriteLine(string.Join(";",new List<string>{bug.HtmlUrl.ToString(),bug.CreatedAt.ToString("d"),bug.User.Login}));
                    }

                }
            }


          

           
        }
    }
}