namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class ConsoleDisplayer:ResultDisplayer
    {
        public override void Display(IEnumerable<string> result)
        {
            result.ToList().ForEach(r => Console.Out.WriteLine(r));
        }
    }
}