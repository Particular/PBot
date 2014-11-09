namespace IssueButler.Mmbot.Caretakers
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using MMBot.Brains;

    public class ManageCaretakers
    {
        public ManageCaretakers(IBrain brain)
        {
            this.brain = brain;

            if (GetActiveRepositories() == null)
            {
                brain.Set(typeof(AvailableRepositories).FullName, new AvailableRepositories())
                    .Wait();
            }
        }

        public void AddCaretaker(string username, string repoName)
        {
            var activeRepositories = GetActiveRepositories();

            var repo = activeRepositories.SingleOrDefault(r => r.Name == repoName);

            if (repo == null)
            {
                throw new Exception("Repository not found, please add it using: mmbot add repo " + repoName);
            }

            repo.Caretaker = username;

            brain.Set(typeof(AvailableRepositories).FullName, activeRepositories)
                .Wait();
        }

        AvailableRepositories GetActiveRepositories()
        {
            var t= brain.Get<AvailableRepositories>(typeof(AvailableRepositories).FullName);

            return t.Result;
        }

        IBrain brain;
    }
}