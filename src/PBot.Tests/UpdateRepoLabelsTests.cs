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
                { "Question", "Type: Discussion" },
                { "Tag: Core v6", "Project: V6 Launch" },
                { "Type: Question", "Type: Discussion" },
                { "Resolution: Can't Reproduce", "Withdrawn: Invalid" },
                { "Resolution: Duplicate", "Withdrawn: Duplicate" },
                { "Resolution: Won't Fix", "Withdrawn: Won't Fix" },
                { "up-for-grabs", "Tag: Up For Grabs" },
                { "Urgent", null },
                { "Type: Breaking Change", "Tag: Breaking Change" },
                { "wontfix", "Withdrawn: Won't Fix" },
            };

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase(
            "Particular",
            new[]
            {
                "AdvancedInstallerLicenser",
                "Backend",
                "GitHubGateway",
                "Loudspeaker",
                "NServiceBus.CodeAnalyzers",
                "NServiceBus.Azure.Samples",
                "Operations.DocsWeb",
                "Operations.Licensing",
                "Operations.Website.Backend",
                "PlatformDevelopment",
                "ProductionTests",
                "ServiceMatrix.Samples",
                "Website.Frontend",
            },
            false)]
        public async Task SyncLabels(string org, string[] privateRepos, bool dryRun)
        {
            Console.Out.WriteLine($"Syncing labels for {org}...");

            var client = GitHubClientBuilder.Build();
            var templateLabels = await client.Issue.Labels.GetAllForRepository(org, "RepoStandards");
            var syncs = (await client.Repository.GetAllForOrg(org))
                .Where(repo => !repo.Private)
                .Select(repo => repo.Name)
                .Concat(privateRepos)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(async repo =>
                {
                    await SyncRepo(client, org, repo, templateLabels, dryRun);
                });

            await Task.WhenAll(syncs);
        }

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular", "TempRepo4PBot", false)]
        public async Task SyncLabels(string org, string repo, bool dryRun)
        {
            var client = GitHubClientBuilder.Build();
            var templateLabels = await client.Issue.Labels.GetAllForRepository(org, "RepoStandards");
            await SyncRepo(client, org, repo, templateLabels, dryRun);
        }

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular", new[] { "Project: V6 Launch" }, false)]
        public async Task SyncLabelsForAllRepos(string org, string[] templateLabelNames, bool dryRun)
        {
            Console.Out.WriteLine($"Syncing labels for {org}...");

            var client = GitHubClientBuilder.Build();
            var templateLabels = (await client.Issue.Labels.GetAllForRepository(org, "RepoStandards"))
                .Where(label => templateLabelNames.Contains(label.Name));

            var syncs = (await client.Repository.GetAllForOrg(org))
                .Select(repo => repo.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(async repo =>
                {
                    try
                    {
                        await SyncRepo(client, org, repo, templateLabels, dryRun);
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine($"Failed to sync {org}/{repo} - {ex.Message}");
                    }
                });

            await Task.WhenAll(syncs);
        }

        private static async Task SyncRepo(
            IGitHubClient client, string org, string repo, IEnumerable<Label> templateLabels, bool dryRun)
        {
            Console.Out.WriteLine($"Syncing labels for {org}/{repo}...");

            var existingLabels = await client.Issue.Labels.GetAllForRepository(org, repo);

            foreach (var templateLabel in templateLabels)
            {
                var existingLabel = existingLabels
                    .SingleOrDefault(l => string.Equals(l.Name, templateLabel.Name, StringComparison.InvariantCultureIgnoreCase));

                if (existingLabel == null)
                {
                    Console.Out.WriteLine($"Creating label '{templateLabel.Name}' in {org}/{repo}...");
                    if (!dryRun)
                    {
                        await client.Issue.Labels.Create(org, repo, new NewLabel(templateLabel.Name, templateLabel.Color));
                    }
                }
                else
                {
                    if (existingLabel.Color != templateLabel.Color ||
                        !existingLabel.Name.Equals(templateLabel.Name, StringComparison.InvariantCulture))
                    {
                        Console.Out.WriteLine($"Fixing color and case for label '{existingLabel.Name}' in {org}/{repo}...");
                        if (!dryRun)
                        {
                            await client.Issue.Labels.Update(
                                org, repo, existingLabel.Name, new LabelUpdate(templateLabel.Name, templateLabel.Color));
                        }
                    }
                }
            }

            existingLabels = (await client.Issue.Labels.GetAllForRepository(org, repo))
                .Concat(dryRun ? templateLabels : Enumerable.Empty<Label>())
                .ToArray();

            foreach (var oldLabel in existingLabels
                .Where(label => labelMap.ContainsKey(label.Name))
                .Select(label => label.Name))
            {
                var newLabel = labelMap[oldLabel];

                var request = new RepositoryIssueRequest { State = ItemState.All };
                request.Labels.Add(oldLabel);
                var issues = await client.Issue.GetAllForRepository(org, repo, request);

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
                        if (existingLabels.Any(label => label.Name == newLabel))
                        {
                            issueUpdate.AddLabel(newLabel);

                            Console.Out.WriteLine(
                                $"Switching from label '{oldLabel}' to '{newLabel}' for {org}/{repo}#{issue.Number}...");

                            if (!dryRun)
                            {
                                await client.Issue.Update(org, repo, issue.Number, issueUpdate);
                            }

                            Console.Out.WriteLine($"Deleting label '{oldLabel}' from {org}/{repo}...");
                            if (!dryRun)
                            {
                                await client.Issue.Labels.Delete(org, repo, oldLabel);
                            }
                        }
                    }
                    else
                    {
                        Console.Out.WriteLine($"Removing label '{oldLabel}' from {org}/{repo}#{issue.Number}...");

                        if (!dryRun)
                        {
                            await client.Issue.Update(org, repo, issue.Number, issueUpdate);
                        }

                        Console.Out.WriteLine($"Deleting label '{oldLabel}' from {org}/{repo}...");
                        if (!dryRun)
                        {
                            await client.Issue.Labels.Delete(org, repo, oldLabel);
                        }
                    }
                }
            }
        }
    }
}