namespace PBot.Tests.Integration
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Octokit;
    using PBot.Requirements;

    public class Feature_development_statistics
    {
        [Test, Explicit]
        public async void GenerateReport()
        {
            var client = GitHubClientBuilder.Build();

            var request = new RepositoryIssueRequest
            {
                Since = DateTimeOffset.Parse("2015-04-12"),
                State = ItemState.All
            };



            var organisation = "Particular";


            var repo = await client.Repository.Get(organisation, "Requirements");

            var requirements = await client.Issue.GetForRepository(organisation, repo.Name, request);
            var activeRequirements = requirements.Where(r => r.State == ItemState.Open).ToList();

            //group by state
            var activeRequirementsGroupedByState = requirements.Where(r => r.State == ItemState.Open)
                .GroupBy(r => r.CurrentState<RequirementStates>()).ToList();

            Console.Out.WriteLine("### Requirement statistics");

            Console.Out.WriteLine("#### By state");

            foreach (var states in activeRequirementsGroupedByState)
            {
                Console.Out.WriteLine("* " + states.Key + ": " + states.Count());
            }

            Console.Out.WriteLine("#### Conserns vs Features");
            var concernCount = activeRequirements.Count(r => r.Labels.Any(l => l.Name == RequirementTypes.Concern));
            Console.Out.WriteLine("{0} - {1}",concernCount, activeRequirements.Count - concernCount);

            Console.Out.WriteLine("#### Completed last period");

            var completedLastPeriod = requirements.Where(r => r.ClosedAt.HasValue && r.ClosedAt.Value > DateTimeOffset.UtcNow - TimeSpan.FromDays(30) && r.Labels.All(l => l.Name != "Closed as won't fix"))
                .ToList();

            foreach (var completed in completedLastPeriod)
            {
                Console.Out.WriteLine("{0} ({1})", completed.HtmlUrl, completed.Labels.Any(l => l.Name == RequirementTypes.Concern)? "Concern":"Feature");     
            }
            Console.Out.WriteLine("Total: " + completedLastPeriod.Count());

        }
    }
}