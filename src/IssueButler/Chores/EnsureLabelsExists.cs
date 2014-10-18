namespace IssueButler.Chores
{
    using System;
    using System.Linq;
    using Octokit;

    public class EnsureLabelsExists : Chore
    { 
        public override void PerformChore(Brain brain)
        {
            var allValidLabels = ClassificationLabels.All.ToList();

            allValidLabels.AddRange(NeedsLabels.All);

            var client = brain.Recall<GitHubClient>();

            var repositories = brain.Recall<RepositoriesToWatchOver>();

            foreach (var repository in repositories)
            {
                var labels = client.Issue.Labels.GetForRepository(repository.Owner.Login, repository.Name).Result;

                foreach (var validLabel in allValidLabels)
                {
                    if (labels.Any(l=>String.Equals(l.Name, validLabel.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        continue;
                    }

                    Console.Out.WriteLine("{0} not found in repo {1}, going to create",validLabel.Name,repository.Name);

                    client.Issue.Labels.Create(repository.Owner.Login, repository.Name, new NewLabel(validLabel.Name, validLabel.Color));

                }
              
            }
        }

    }
}