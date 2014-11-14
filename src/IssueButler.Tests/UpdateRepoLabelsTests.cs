namespace IssueButler.Tests
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot;
    using NUnit.Framework;
    using Octokit;

    [TestFixture]
    public class UpdateRepoLabelsTests
    {
        const string Organization = "Particular";

        [Test, Explicit("Performs the actual sync for now")]
        public void SyncAllRepos()
        {
            var client = GitHubClientBuilder.Build();
                   var sourceRepo = "NServiceBus";

            
            var labelsToSync = client.Issue.Labels.GetForRepository(Organization, sourceRepo).Result;


            var repoToUpdate = "RepoStandards";
            var existingLabels = client.Issue.Labels.GetForRepository(Organization, repoToUpdate).Result;

            
            foreach (var label in labelsToSync)
            {
                if (!existingLabels.Any(existing => existing.Name == label.Name))
                {
                    Console.Out.WriteLine("{0} doesn't exist and will be created",label.Name);
                    client.Issue.Labels.Create(Organization, repoToUpdate, new NewLabel(label.Name, label.Color)).Wait();
                }
            }
     
        }
    }
}