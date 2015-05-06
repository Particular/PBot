namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;

    public class Issue_statistics
    {
        [Test, Explicit]
        public async void AllBugs()
        {
            
            await GenerateReport(bugsOnly: true)
                ;
        }

        [Test, Explicit]
        public async void ByExternalUsers()
        {

            await GenerateReport(byExternalUsers: true)
                ;
        }
        public async Task GenerateReport(bool bugsOnly = false,bool byExternalUsers = false)
        {
            var client = GitHubClientBuilder.Build();

            var request = new RepositoryIssueRequest
            {
                Since = DateTimeOffset.Parse("2014-01-01")
            };

            //  request.Labels.Add("Bug");


            var organisation = "Particular";

            var members = await client.Organization.Member.GetAll(organisation);

            var repos = await client.Repository.GetAllForOrg(organisation);

            foreach (var repo in repos.Where(r=>!r.Private))
            {
                var issues = await client.Issue.GetForRepository(organisation, repo.Name, request);
                foreach (var issue in issues)
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