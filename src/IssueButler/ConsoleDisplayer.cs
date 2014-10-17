namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class ConsoleDisplayer:ResultDisplayer
    {
        public override void Display(IEnumerable<ValidationError> result)
        {

            result.ToList().ForEach(r =>
            {
                var message =  string.Format("{0} - {1}", r.Issue.HtmlUrl, r.Reason);
     
                Console.Out.WriteLine(message);
            });
        }
    }
}
