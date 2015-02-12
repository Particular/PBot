namespace PBot.Reminders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using NuGet;
    using Octokit;
    using PBot.Users;

    public class RemindUsersOfBugsWithMissingSections : BotCommand
    {
        public RemindUsersOfBugsWithMissingSections()
            : base(
                "remind users of mandatory bug sections$",
                "pbot remind users of mandatory bug sections - Checks and reminds users to put required sections on issues marked as bugs")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();


            await response.Send("Found the following repos: ");
            foreach (var repo in Brain.Get<AvailableRepositories>())
            {
                await response.Send(repo.Name);
            }

            foreach (var repo in Brain.Get<AvailableRepositories>())
            {
                await response.Send("Checking " + repo.Name);
                await Check(client, response, repo.Name);               
            }
        }

        async Task Check(GitHubClient client, IResponse response, string name)
        {
            var issueFilter = new RepositoryIssueRequest
            {
                State = ItemState.Closed,
                Since = DateTimeOffset.Parse("2014-11-01") //cutoff date
            };

            issueFilter.Labels.Add("Bug");

            var bugs = await client.Issue.GetForRepository("Particular", name, issueFilter);


            foreach (var bug in bugs)
            {
                if (bug.Milestone == null)
                {
                    continue;
                }

                if (bug.Milestone.State == ItemState.Closed)
                {
                    continue;
                }


                var hasAffected = bug.Body.Contains("## Who's affected");
                var hasSymptoms = bug.Body.Contains("## Symptoms");

                if (hasAffected && hasSymptoms)
                {
                    continue;
                }

                //todo: need to patch octokit to support ClosedBy
                var events = await client.Issue.Events.GetForIssue("Particular", name, bug.Number);



                var closedEvent = events.First(e => e.Event == EventInfoState.Closed);

                var userThatClosedTheIssue = closedEvent.Actor.Login;

                string introText;


                var chatUsername = Brain.Get<CredentialStore>()
                    .Where(s => s.Credentials.Any(credential => credential.Name == "github-username" && credential.Value == userThatClosedTheIssue))
                    .Select(c => c.Username)
                    .SingleOrDefault();

                if (chatUsername == null)
                {
                    introText = string.Format("Hi @channel! The github user {0} closed this issue and I can't seem to find what slack username that maps to. Would you please remind the user that", userThatClosedTheIssue);
                }
                else
                {
                    introText = string.Format("Hi there @{0}! I've seen that", chatUsername);
                }

                await response.Send(string.Format("{0} this bug is missing some mandatory sections. Please head over to {1} and update it! Read more on required sections here: {2}",
                    introText,
                    bug.HtmlUrl,
                    @"https://github.com/Particular/Housekeeping/wiki/Required-sections-for-bugs")).IgnoreWaitContext();


                if (chatUsername == null)
                {
                    await response.Send(string.Format("Do you know has the GitHub username {0} ? Do me a favor and ask them to register their username using: `pbot register credential github-username={0}` so that I can get in touch with them. Thanks!", userThatClosedTheIssue));
                }


            }
        }
    }
}