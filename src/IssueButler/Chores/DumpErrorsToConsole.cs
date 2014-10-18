namespace IssueButler
{
    using System;
    using System.Linq;

    class DumpErrorsToConsole:Chore
    {

        public override void PerformChore(Brain brain)
        {
            var errors = brain.Recall<ValidationErrors>();

            errors.ToList().ForEach(r =>
            {
                var message =  string.Format("{0} - {1}", r.Issue.HtmlUrl, r.Reason);
     
                Console.Out.WriteLine(message);
            });
        }

    }
}
