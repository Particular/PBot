namespace PBot.Caretakers
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;

    public class ListCaretakers : BotCommand
    {
        public ListCaretakers()
            : base(
                "list caretakers$",
                "pbot list caretakers - Displays the list of caretakers") { }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var groupByCaretaker = Brain.Get<AvailableRepositories>()
                .GroupBy(r => r.Caretaker)
                .ToList();

            foreach (var caretaker in groupByCaretaker.Where(g => g.Key != null))
            {
                await response.Send(string.Format("*{0}*: {1}", caretaker.Key, string.Join(", ", caretaker.Select(r => r.Name))));
            }
            var upForGrabs = groupByCaretaker.SingleOrDefault(g => g.Key == null);

            if (upForGrabs != null)
            {
                await response.Send("Repos that is up for grabs:").IgnoreWaitContext();
                await response.Send(string.Join(", ", upForGrabs.Select(r => r.Name))).IgnoreWaitContext();
            }

            await response.Send("You can sign up using: `pbot register {your username} as caretaker for {name of the repo above}`").IgnoreWaitContext();
            await response.Send("Please read more about caretakers here - https://github.com/Particular/Housekeeping/wiki/Caretakers").IgnoreWaitContext();
        }
    }
}