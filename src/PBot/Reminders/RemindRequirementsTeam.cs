namespace PBot.Reminders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class RemindRequirementsTeam : BotCommand
    {
        public RemindRequirementsTeam()
            : base(
                "remind requirement team",
                "pbot remind requirement team - Reminds the requirements team when there is issues in https://github.com/Particular/Requirements that needs attention")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();

            var issueFilter = new RepositoryIssueRequest
            {
                State = ItemState.Open
            };

            var issues = await client.Issue.GetForRepository("Particular", "Requirements", issueFilter);

            var actionsNeeded = new List<string>();

            var numApproved = issues.Count(i => i.Labels.Any(l => l.Name == "State: Prioritized"));

            if (numApproved < 2)
            {
                actionsNeeded.Add(string.Format("- There is only {0} approved requirements, please move more to the `Approved` swimlane!",numApproved));
            }
            
            var numImpactAssesement = issues.Count(i => i.Labels.Any(l => l.Name == "Needs: Impact Assessment"));

            if (numImpactAssesement < 5)
            {
                actionsNeeded.Add(string.Format("- There is only {0} card in the `Define: Customer Impact & Importance` swimlane, let's get more high prio requirements over there?",numImpactAssesement));

            }

            //check for cards in the discuss swimland that is getting stale
            issues.Where(i=>i.Labels.Any(l=>l.Name=="Needs: Discussion") && i.UpdatedAt < DateTime.Today.AddDays(-5)).ToList()
                .ForEach(i => actionsNeeded.Add(string.Format("- {0} has been in the `Needs: Discussion` state for more than a week. Lets try to make a decision? ",i.HtmlUrl)));

            if (actionsNeeded.Any())
            {
                await response.Send(string.Format("@channel Hi there! I've noticed that there is few things that needs human attention:")).IgnoreWaitContext();

                foreach (var action in actionsNeeded)
                {
                    await response.Send(action).IgnoreWaitContext();
                }

                await response.Send(string.Format("You can read more on how the feature development process works here: {0}", @"https://github.com/Particular/Strategy/issues/5")).IgnoreWaitContext();

            }

        }
    }
}