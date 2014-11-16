namespace IssueButler.Tests
{
    using System;
    using System.Collections.Generic;
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
            var sourceRepo = "RepoStandards";


            var labelsToSync = client.Issue.Labels.GetForRepository(Organization, sourceRepo).Result;

            //go through all repos
            var repos = client.Repository.GetAllForOrg(Organization).Result;

            foreach (var repository in repos)
            {
                Console.Out.WriteLine("-------- Checking {0} -----", repository.Name);
                SyncRepo(client, repository.Name, labelsToSync);
            }





        }

        static void SyncRepo(GitHubClient client, string repoToUpdate, IReadOnlyList<Label> labelsToSync)
        {
            var existingLabels = client.Issue.Labels.GetForRepository(Organization, repoToUpdate).Result;


            foreach (var templateLabel in labelsToSync)
            {
                var existingLabel = existingLabels.SingleOrDefault(l => l.Name == templateLabel.Name);

                if (existingLabel == null)
                {
                    Console.Out.WriteLine("{0} doesn't exist and will be created", templateLabel.Name);
                    client.Issue.Labels.Create(Organization, repoToUpdate, new NewLabel(templateLabel.Name, templateLabel.Color)).Wait();
                }
                else
                {
                    if (existingLabel.Color != templateLabel.Color)
                    {
                        Console.Out.WriteLine("{0} has non matching color, will be updated", existingLabel.Name);
                        client.Issue.Labels.Update(Organization, repoToUpdate, existingLabel.Name, new LabelUpdate(templateLabel.Name, templateLabel.Color))
                            .Wait();
                    }
                }
            }

            var nonStandardLabels = existingLabels.Where(l => !labelsToSync.Any(template => template.Name == l.Name)).ToList();

            //Needs: Hotfix is a non standard label
            //Needs: Investigation is a non standard label
            var blacklistedLabels = new[]
            {
                "duplicate",
                "enhancement",
                "help wanted",
                "invalid",
                "wontfix",
                "Needs: Hotfix",
                "Needs: Investigation"
            };
            foreach (var nonStandardLabel in nonStandardLabels)
            {
                Console.Out.WriteLine("{0} is a non standard label", nonStandardLabel.Name);

                if (blacklistedLabels.Any(b => b == nonStandardLabel.Name))
                {
                    Console.Out.WriteLine("{0} is a blacklisted and will be removed", nonStandardLabel.Name);

                    client.Issue.Labels.Delete(Organization, repoToUpdate, nonStandardLabel.Name);
                }
            }
        }
    }
}