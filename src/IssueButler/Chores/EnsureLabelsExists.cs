namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class EnsureLabelsExists : Validator
    {
        GitHubClient client;
        public EnsureLabelsExists()
        {
            client = GitHubClientBuilder.Build();
        }

        public override IEnumerable<ValidationError> Validate(IEnumerable<Repository> repositories)
        {
            var allValidLabels = ClassificationLabels.All;


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

            return new List<ValidationError>();
        }
    }
}