namespace PBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using PBot;
    using PBot.Issues;

    [TestFixture]
    public class UpdateRepoLabelsTests
    {
        private static readonly Dictionary<string, string> labelMap =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "Breaking Change", "Tag: Breaking Change" },
                { "Bug", "Type: Bug" },
                { "Critical", null },
                { "duplicate", "Withdrawn: Duplicate" },
                { "enhancement", "Type: Feature" },
                { "Feature", "Type: Feature" },
                { "help wanted", null },
                { "Improvement", "Type: Feature" },
                { "in progress", "State: In Progress" },
                { "Internal Refactoring", ClassificationLabels.RefactoringLabelName },
                { "invalid", "Withdrawn: Invalid" },
                { "Question", "Type: Question" },
                { "Resolution: Can't Reproduce", "Withdrawn: Invalid" },
                { "Resolution: Duplicate", "Withdrawn: Duplicate" },
                { "Resolution: Won't Fix", "Withdrawn: Won't Fix" },
                { "State: Approved", null },
                { "State: Prioritized", null },
                { "State: Review", null },
                { "Urgent", null },
                { "Type: Breaking Change", "Tag: Breaking Change" },
                { "wontfix", "Withdrawn: Won't Fix" },
            };

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular", new[] { "Operations.Licensing", "PlatformDevelopment", "ProductionTests" })]
        public async void SyncLabels(string org, string[] privateRepos)
        {
            Console.Out.WriteLine($"Syncing labels for {org}...");

            var client = GitHubClientBuilder.Build();
            var templateLabels = await client.Issue.Labels.GetForRepository(org, "RepoStandards");
            var syncs = (await client.Repository.GetAllForOrg(org))
                .Where(repo => !repo.Private)
                .Select(repo => repo.Name)
                .Concat(privateRepos)
                .Select(async repo =>
            {
                await SyncRepo(client, org, repo, templateLabels);
            });

            await Task.WhenAll(syncs);
        }

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular", "TempRepo4PBot")]
        public async void SyncLabels(string org, string repo)
        {
            var client = GitHubClientBuilder.Build();
            var templateLabels = await client.Issue.Labels.GetForRepository(org, "RepoStandards");
            await SyncRepo(client, org, repo, templateLabels);
        }

        private static async Task SyncRepo(IGitHubClient client, string org, string repo, IEnumerable<Label> templateLabels)
        {
            Console.Out.WriteLine($"Syncing labels for {org}/{repo}...");

            var existingLabels = await client.Issue.Labels.GetForRepository(org, repo);

            foreach (var templateLabel in templateLabels)
            {
                var existingLabel = existingLabels
                    .SingleOrDefault(l => string.Equals(l.Name, templateLabel.Name, StringComparison.InvariantCultureIgnoreCase));

                if (existingLabel == null)
                {
                    Console.Out.WriteLine($"Creating label '{templateLabel.Name}' in {org}/{repo}...");
                    await client.Issue.Labels.Create(org, repo, new NewLabel(templateLabel.Name, templateLabel.Color));
                }
                else
                {
                    if (existingLabel.Color != templateLabel.Color ||
                        !existingLabel.Name.Equals(templateLabel.Name, StringComparison.InvariantCulture))
                    {
                        Console.Out.WriteLine($"Fixing color and case for label '{existingLabel.Name}' in {org}/{repo}...");
                        await client.Issue.Labels.Update(
                            org, repo, existingLabel.Name, new LabelUpdate(templateLabel.Name, templateLabel.Color));
                    }
                }
            }

            foreach (var oldLabel in existingLabels
                .Where(label => labelMap.ContainsKey(label.Name))
                .Select(label => label.Name))
            {
                var newLabel = labelMap[oldLabel];

                var request = new RepositoryIssueRequest { State = ItemState.All };
                request.Labels.Add(oldLabel);
                var issues = await client.Issue.GetForRepository(org, repo, request);

                foreach (var issue in issues)
                {
                    // NOTE (adamralph): there is a bug in Octokit or the GitHub API
                    // that removes the assignee unless set (perhaps just for private repos)
                    var issueUpdate = new IssueUpdate
                    {
                        Assignee = issue.Assignee?.Login,
                    };

                    foreach (var label in issue.Labels.Where(label => label.Name != oldLabel))
                    {
                        issueUpdate.AddLabel(label.Name);
                    }

                    if (newLabel != null)
                    {
                        issueUpdate.AddLabel(newLabel);
                        Console.Out.WriteLine(
                            $"Switching from label '{oldLabel}' to '{newLabel}' for {org}/{repo}#{issue.Number}...");
                    }
                    else
                    {
                        Console.Out.WriteLine($"Removing label '{oldLabel}' from {org}/{repo}#{issue.Number}...");
                    }

                    await client.Issue.Update(org, repo, issue.Number, issueUpdate);
                }

                Console.Out.WriteLine($"Deleting label '{oldLabel}' from {org}/{repo}...");
                await client.Issue.Labels.Delete(org, repo, oldLabel);
            }
        }
    }
}
