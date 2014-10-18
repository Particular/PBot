namespace IssueButler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class EnsureLabelsExists : Chore
    { 
        public override void PerformChore(Brain brain)
        {
            var allValidLabels = ClassificationLabels.All;

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

    public class RepositoriesToWatchOver:IEnumerable<Repository>
    {
        readonly IEnumerable<Repository> repositories;

        public RepositoriesToWatchOver(IEnumerable<Repository> repositories)
        {
            this.repositories = repositories;
        }

        public IEnumerator<Repository> GetEnumerator()
        {
            return repositories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class Chore
    {
        public  abstract void PerformChore(Brain brain);

        public virtual string Description
        {
            get { return GetType().FullName; }
        }
    }

    public class Brain
    {
        public void Remember<T>(T thingToRemember)
        {
            memory[typeof(T).FullName] = thingToRemember;
        }

        public T Recall<T>()
        {
            return (T) memory[typeof(T).FullName];
        }

        Dictionary<string,object> memory = new Dictionary<string, object>(); 
    }
}