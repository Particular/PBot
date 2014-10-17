namespace IssueButler
{
    using System.Collections.Generic;

    public abstract class ResultDisplayer
    {
        public abstract void Display(IEnumerable<ValidationError> result);
    }
}