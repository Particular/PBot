namespace PBot.Tests.Integration
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using Octokit;

    public class List_my_TaskForces //: BotCommandFixture<ListIssuesFromExternalContributors>
    {
        [Test, Explicit]
        public async void All()
        {
            Console.Out.WriteLine( "### My Task forces" );
            var sw = Stopwatch.StartNew();

            var client = GitHubClientBuilder.Build();

            var username = "mauroservienti";
            var filter = new RepositoryIssueRequest
            {
                State = ItemState.Open,
                Mentioned = username
            };

            foreach( var repo in await client.Repository.GetAllForOrg( "Particular" ) )
            {
                var issues = await client.Issue.GetForRepository( "Particular", repo.Name, filter );

                var regex = new Regex( @"Task[\s-]?Force:\s*(.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled );

                var inTaskForce = (
                    from issue in issues
                    where issue.Body != null
                    let match = regex.Match( issue.Body )
                    where match.Success
                    let taskForce = Regex.Matches( match.Result( "$1" ), "@[a-z0-9.-]+" ).Cast<Match>().Select( x => x.Value.TrimStart( '@' ) )
                    let isPig = taskForce.Contains( username, StringComparer.InvariantCultureIgnoreCase )
                    orderby isPig descending, repo.Name ascending
                    select new
                    {
                        Repo = repo.Name,
                        IssueUrl = issue.HtmlUrl.ToString(),
                        IssueTitle = issue.Title,
                        Involvement = isPig ? "PIG" : "chicken",
                        Labels = issue.Labels.Select( x => x.Name ).ToArray(),
                        Team = taskForce.ToArray()
                    }
                ).ToList();

                foreach( var item in inTaskForce.Where( a => a.Team.Contains( username ) ).GroupBy( a => a.Repo ) )
                {
                    Console.Out.WriteLine( "*[{0}]*", item.Key );
                    foreach( var issue in item )
                    {
                        Console.Out.WriteLine( "\t- _{0}_ ({1})", issue.IssueTitle, issue.IssueUrl );
                        Console.Out.WriteLine( "\tLabels: {0}", string.Join( " ", issue.Labels.Select( l => "`" + l + "`" ) ) );
                        Console.Out.WriteLine( "\tTeam: {0}", string.Join( ", ", issue.Team ) );
                    }
                }
            }

            sw.Stop();
            Console.Out.WriteLine( "Total time (in seconds): {0}", sw.Elapsed.TotalSeconds );
        }
    }
}