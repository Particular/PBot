namespace IssueButler.Mmbot.Caretakers
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;

    public class ListCaretakers : BotCommand
    {
        public ListCaretakers()
            : base(
                "list caretakers$",
                "pbot list caretakers - Displays the list of caretakers") { }

        public override void Execute(string[] parameters, IResponse response)
        {
            var activeRepositories = Brain.Get<AvailableRepositories>();


            foreach (var repo in activeRepositories.Where(r=>r.Caretaker != null))
            {
                response.Send(string.Format("{0} is caretaker for {1}",repo.Caretaker,repo.Name));
            }
            var upForGrabs = activeRepositories.Where(r => r.Caretaker == null).ToList();

            if (upForGrabs.Any())
            {
                response.Send(string.Format("Repos that is up for grabs: {0} for more info on what a caretaker is see {1}", string.Join(", ", upForGrabs.Select(r => r.Name)), @"https://github.com/Particular/Housekeeping/wiki/Caretakers"));              
            }
            
        }
    }
}