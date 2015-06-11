namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using PBot.Issues;

    public class Issue_statistics:BotCommandFixture<ListIssuesFromExternalContributors>
    {
        [Test, Explicit]
        public async void AllBugs()
        {
            Console.Out.WriteLine("### All bugs");
            await GenerateReport(bugsOnly: true)
                ;
        }

        [Test, Explicit]
        public async void ByExternalUsers()
        {
           await Execute("","2015-05-01 to 2015-06-01");
        }

        public async Task GenerateReport(bool bugsOnly = false,bool byExternalUsers = false)
        {
            var client = GitHubClientBuilder.Build();

            var period = DateTimeOffset.Parse("2015-05-01");

            var request = new RepositoryIssueRequest
            {
                Since = period
            };

            //  request.Labels.Add("Bug");


            var organisation = "Particular";

            var members = await client.Organization.Member.GetAll(organisation);

            var repos = await client.Repository.GetAllForOrg(organisation);

            foreach (var repo in repos.Where(r=>!r.Private))
            {
                var issues = await client.Issue.GetForRepository(organisation, repo.Name, request);
                foreach (var issue in issues.Where(i => i.CreatedAt >= period))
                {
                    var createdByExternalUser = members.All(m => m.Login != issue.User.Login);
                    var isBug = issue.Labels.Any(l => l.Name.ToLower() == "bug");


                    if (bugsOnly && isBug)
                    {
                        Console.Out.WriteLine(string.Join(";", new List<string> { issue.HtmlUrl.ToString(), issue.CreatedAt.ToString("d"),createdByExternalUser.ToString(), issue.User.Login }));             
                    }

                    if (byExternalUsers && createdByExternalUser)
                    {
                        Console.Out.WriteLine(string.Join(";", new List<string> { issue.HtmlUrl.ToString(), issue.CreatedAt.ToString("d"),isBug.ToString(), issue.User.Login }));
                    }
           
                }
            }


          

           
        }
    }
}