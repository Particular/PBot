namespace IssueButler
{
    using System.Collections.Generic;

    public abstract class Validator
    {
        public abstract IEnumerable<ValidationError> Validate();
    }
}