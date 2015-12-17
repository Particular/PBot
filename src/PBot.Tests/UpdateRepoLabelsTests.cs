namespace PBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                { "Breaking Change", "Type: Breaking Change" },
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
                { "wontfix", "Withdrawn: Won't Fix" },
            };

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular")]
        public void SyncLabels(string org)
        {
            Console.Out.WriteLine($"Syncing labels for {org}...");

            var client = GitHubClientBuilder.Build();
            var templateLabels = client.Issue.Labels.GetForRepository(org, "RepoStandards").Result;

            var repos = new[]
            {
                "AutomaticOctopusConfigSandbox",
                "Automation.Engineering",
                "Automation.ITOps",
                "Automation.Marketing",
                "BuildProcess.StandardNuGet",
                "ConsoleTweet",
                "DeploymentProcess.BuildTools",
                "DeploymentProcess.DeploymentTools",
                "DeploymentProcess.InternalMyGet",
                "DeploymentProcess.OperationsContracts",
                "DeploymentProcess.StandardNuGet",
                "DeploymentSandbox",
                "GitHubReleaseNotes",
                "HashBus",
                "mmbot",
                "NServiceBus",
                "NServiceBus.Autofac",
                "NServiceBus.Azure",
                "NServiceBus.Azure.Samples",
                "NServiceBus.AzureBlobStorageDataBus",
                "NServiceBus.AzureServiceBus",
                "NServiceBus.AzureStoragePersistence",
                "NServiceBus.AzureStorageQueues",
                "NServiceBus.Bootstrap.WindowsService",
                "NServiceBus.Callbacks",
                "NServiceBus.CastleWindsor",
                "NServiceBus.CommonLogging",
                "NServiceBus.Distributor.Msmq",
                "NServiceBus.Gateway",
                "NServiceBus.Host",
                "NServiceBus.Host.AzureCloudService",
                "NServiceBus.Log4Net",
                "NServiceBus.Newtonsoft.Json",
                "NServiceBus.NHibernate",
                "NServiceBus.Ninject",
                "NServiceBus.NLog",
                "NServiceBus.PowerShell",
                "NServiceBus.RabbitMQ",
                "NServiceBus.RavenDB",
                "NServiceBus.ServiceFabric",
                "NServiceBus.Spring",
                "NServiceBus.SqlServer",
                "NServiceBus.StructureMap",
                "NServiceBus.Testing",
                "NServiceBus.Unity",
                "NuGetPackager",
                "OctopusConfigUpdater",
                "OctopusProjectUpdater",
                "OctopusWrapper",
                "Packages.DTC",
                "Packages.Msmq",
                "Packages.PerfCounters",
                "PBot",
                "PBot.TestRepo",
                "PlatformInstaller",
                "RepoStandards",
                "ServiceControl",
                "ServiceControl.Contracts",
                "ServiceControl.Plugin.Nsb3.CustomChecks",
                "ServiceControl.Plugin.Nsb3.Heartbeat",
                "ServiceControl.Plugin.Nsb4.CustomChecks",
                "ServiceControl.Plugin.Nsb4.DebugSession",
                "ServiceControl.Plugin.Nsb4.Heartbeat",
                "ServiceControl.Plugin.Nsb4.SagaAudit",
                "ServiceControl.Plugin.Nsb5.CustomChecks",
                "ServiceControl.Plugin.Nsb5.DebugSession",
                "ServiceControl.Plugin.Nsb5.Heartbeat",
                "ServiceControl.Plugin.Nsb5.SagaAudit",
                "ServiceControl.Plugins.v4",
                "ServiceControl.Plugins.v5",
                "ServiceInsight",
                "ServiceMatrix",
                "ServiceMatrix.Samples",
                "ServicePulse",
                "SignVSIX",
                "SyncOMatic",
                "SyncOMatic.TestRepository",
                "TeamCityProjectCreator",
                "Timestamp",
                "Topshelf",
            };

            foreach (var repo in repos)
            {
                SyncRepo(client, org, repo, templateLabels);
            }
        }

        [Test, Explicit("Performs the actual sync for now")]
        [TestCase("Particular", "TempRepo4PBot")]
        public void SyncLabels(string org, string repo)
        {
            var client = GitHubClientBuilder.Build();
            var templateLabels = client.Issue.Labels.GetForRepository(org, "RepoStandards").Result;
            SyncRepo(client, org, repo, templateLabels);
        }

        private static void SyncRepo(IGitHubClient client, string org, string repo, IEnumerable<Label> templateLabels)
        {
            Console.Out.WriteLine($"Syncing labels for {org}/{repo}...");

            var existingLabels = client.Issue.Labels.GetForRepository(org, repo).Result;

            foreach (var templateLabel in templateLabels)
            {
                var existingLabel = existingLabels
                    .SingleOrDefault(l => string.Equals(l.Name, templateLabel.Name, StringComparison.InvariantCultureIgnoreCase));

                if (existingLabel == null)
                {
                    Console.Out.WriteLine("{0} doesn't exist and will be created", templateLabel.Name);
                    client.Issue.Labels.Create(org, repo, new NewLabel(templateLabel.Name, templateLabel.Color)).Wait();
                }
                else
                {
                    if (existingLabel.Color != templateLabel.Color ||
                        !existingLabel.Name.Equals(templateLabel.Name, StringComparison.InvariantCulture))
                    {
                        Console.Out.WriteLine("{0} has non matching color or case, will be updated", existingLabel.Name);
                        client.Issue.Labels.Update(
                                org, repo, existingLabel.Name, new LabelUpdate(templateLabel.Name, templateLabel.Color))
                            .Wait();
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
                var issues = client.Issue.GetForRepository(org, repo, request).Result;

                foreach (var issue in issues)
                {
                    var issueUpdate = new IssueUpdate();
                    foreach (var label in issue.Labels.Where(label => label.Name != oldLabel))
                    {
                        issueUpdate.AddLabel(label.Name);
                    }

                    if (newLabel != null)
                    {
                        issueUpdate.AddLabel(newLabel);
                        Console.Out.WriteLine(
                            "Switching from label '{0}' to '{1}' for issue {2} in repo '{3}'.",
                            oldLabel,
                            newLabel,
                            issue.Number,
                            repo);
                    }
                    else
                    {
                        Console.Out.WriteLine(
                            "Removing label '{0}' from issue {1} in repo '{2}'.", oldLabel, issue.Number, repo);
                    }

                    client.Issue.Update(org, repo, issue.Number, issueUpdate).Wait();
                }

                Console.Out.WriteLine("Removing label '{0}' from repo '{1}'.", oldLabel, repo);
                client.Issue.Labels.Delete(org, repo, oldLabel).Wait();
            }
        }
    }
}
