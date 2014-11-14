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

            
            foreach (var templateLabel in labelsToSync)
            {
                var existingLabel = existingLabels.SingleOrDefault(l => l.Name == templateLabel.Name);

                if (existingLabel == null)
                {
                    Console.Out.WriteLine("{0} doesn't exist and will be created",templateLabel.Name);
                    client.Issue.Labels.Create(Organization, repoToUpdate, new NewLabel(templateLabel.Name, templateLabel.Color)).Wait();
                }
                else
                {
                    if (existingLabel.Color != templateLabel.Color)
                    {
                        Console.Out.WriteLine("{0} has non matching color, will be updated",existingLabel.Name);
                        client.Issue.Labels.Update(Organization, repoToUpdate,existingLabel.Name, new LabelUpdate(templateLabel.Name, templateLabel.Color))
                            .Wait();
                    }
                }

                //
            }
     
        }
    }
}