namespace IssueButler.Mmbot.Caretakers
{
    using System.Linq;
    using System.Text;
    using IssueButler.Mmbot.Repositories;

    public class ListCaretakers : BotCommand
    {
        public ListCaretakers()
            : base(
                "list caretakers$",
                "pbot list caretakers - Displays the list of caretakers") { }

        public override void Execute(string[] parameters, IResponse response)
        {
            var groupByCaretaker = Brain.Get<AvailableRepositories>()
                .GroupBy(r => r.Caretaker)
                .ToList();

            var message = new StringBuilder();

            foreach (var caretaker in groupByCaretaker.Where(g => g.Key != null))
            {
                message.AppendLine(string.Format("*{0}*: {1}", caretaker.Key, string.Join(", ", caretaker.Select(r => r.Name))));
            }
            var upForGrabs = groupByCaretaker.SingleOrDefault(g => g.Key == null);

            if (upForGrabs != null)
            {
                message.AppendLine("Repos that is up for grabs:");
                message.AppendLine(string.Join(", ", upForGrabs.Select(r => r.Name)));
            }

            message.AppendLine("You can sign up using: `pbot register {your username} as caretaker for {name of the repo above}`");
            message.AppendLine("Please read more about caretakers here - https://github.com/Particular/Housekeeping/wiki/Caretakers");

            response.Send(message.ToString());
        }
    }
}