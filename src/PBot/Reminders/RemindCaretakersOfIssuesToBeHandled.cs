namespace PBot.Reminders
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using PBot.Issues;

    public class RemindCaretakersOfIssuesToBeHandled : BotCommand
    {
        public RemindCaretakersOfIssuesToBeHandled()
            : base(
                "remind caretakers of issues$",
                "`pbot remind caretakers of issues` - Send a private message to each caretaker if there are issues to be handled") { }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var reposGroupedByCaretaker = Brain.Get<AvailableRepositories>()
                .Where(r => r.Caretaker != null)
                .GroupBy(r => r.Caretaker)
                .ToList();

            var client = GitHubClientBuilder.Build();

            foreach (var repoGroup in reposGroupedByCaretaker)
            {
                var validationErrors = repoGroup.SelectMany(r =>
                {
                    var repo = client.Repository.Get("Particular", r.Name).Result;

                    return new CheckIssuesForRepository(repo, client)
                        .Execute();
                })
                .ToList();

                if (!validationErrors.Any())
                {
                    continue;
                }

                await response.Send($"Hi there @{repoGroup.Key}! There is a few issues in {string.Join(", ", repoGroup)} that needs some attention and since you're the caretaker I thought you would like to be aware. Just type `pbot check my repos` if you want me to help you getting started!").IgnoreWaitContext();
            }
        }
    }
}
