namespace IssueButler.Mmbot.Reminders
{
    using System.Linq;
    using IssueButler.Mmbot.Issues;
    using IssueButler.Mmbot.Repositories;

    public class RemindCaretakersOfIssuesToBeHandled : BotCommand
    {
        public RemindCaretakersOfIssuesToBeHandled()
            : base(
                "remind caretakers of issues if needed$",
                "pbot remind caretakers of issues if needed - Send a private message to each caretaker if there are issues to be handled") { }

        public override void Execute(string[] parameters, IResponse response)
        {
            var reposWithACaretaker = Brain.Get<AvailableRepositories>()
                .Where(r => r.Caretaker != null)
                .ToList();

            var client = GitHubClientBuilder.Build();

            foreach (var repo in reposWithACaretaker)
            {
                var ghRepo = client.Repository.Get("Particular", repo.Name).Result;

                var validationErrors = new CheckIssuesForRepository(ghRepo, client)
                    .Execute();

                if (!validationErrors.Any())
                {
                    continue;
                }

                //todo: soon to be a private message
                response.Send(string.Format("Hi there {1}! There is a few issues in {0} that needs some attention and since you're the caretaker I thought you would like to be aware. Just type pbot check repo {0} if you want me to help you getting started!", repo.Name, repo.Caretaker));
            }
        }
    }
}