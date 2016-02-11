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
        public async System.Threading.Tasks.Task GenerateReport()
        {
            var client = GitHubClientBuilder.Build();
            var period = TimeSpan.FromDays(30);

            var request = new RepositoryIssueRequest
            {
                State = ItemState.All
            };

            var organisation = "Particular";

            var repo = await client.Repository.Get(organisation, "PlatformDevelopment");

            var requirements = await client.Issue.GetAllForRepository(organisation, repo.Name, request);
            var activeRequirements = requirements.Where(r => r.State == ItemState.Open).ToList();

            //group by state
            var activeRequirementsGroupedByState = requirements.Where(r => r.State == ItemState.Open)
                .GroupBy(r => r.CurrentState<RequirementStates>()).ToList();

            Console.Out.WriteLine("## By state");

            foreach (var states in activeRequirementsGroupedByState)
            {
                Console.Out.WriteLine("* " + states.Key + ": " + states.Count());
            }

            Console.Out.WriteLine("## Concerns vs Features");
            var concernCount = activeRequirements.Count(r => r.Labels.Any(l => l.Name == RequirementTypes.Concern));
            Console.Out.WriteLine("* {0} - {1}", concernCount, activeRequirements.Count - concernCount);

            var newLastPeriod = requirements.Where(r => !r.ClosedAt.HasValue && r.CreatedAt > DateTimeOffset.UtcNow - period)
                .ToList();

            Console.Out.WriteLine("## New last week ({0})", newLastPeriod.Count);

            foreach (var newIssue in newLastPeriod)
            {
                Console.Out.WriteLine("* [{0}]({1}) ({2})", newIssue.Title, newIssue.HtmlUrl, newIssue.Labels.Any(l => l.Name == RequirementTypes.Concern) ? "Concern" : "Feature");
            }
            Console.Out.WriteLine("Total: " + newLastPeriod.Count());

            var completedLastPeriod = requirements.Where(r => r.ClosedAt.HasValue && r.ClosedAt.Value > DateTimeOffset.UtcNow - period && r.Labels.All(l => l.Name != "Closed as won't do"))
                .ToList();

            Console.Out.WriteLine("## Completed last week ({0})", completedLastPeriod.Count);

            foreach (var completed in completedLastPeriod)
            {
                Console.Out.WriteLine("* [{0}]({1}) ({2})", completed.Title, completed.HtmlUrl, completed.Labels.Any(l => l.Name == RequirementTypes.Concern) ? "Concern" : "Feature");
            }

            var nonAligned = activeRequirements.Where(r => !r.Body.Contains("# Alignment with vision"))
                .ToList();
            Console.Out.WriteLine("## Issues with no alignment with vision ({0}%)", Math.Round(Convert.ToDouble(nonAligned.Count) / Convert.ToDouble(activeRequirements.Count) * 100.0));

            foreach (var nonAlignedIssue in nonAligned)
            {
                Console.Out.WriteLine("* [{0}]({1})", nonAlignedIssue.Title, nonAlignedIssue.HtmlUrl);
            }
        }
    }
}