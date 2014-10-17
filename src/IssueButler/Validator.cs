namespace IssueButler
{
    using System.Collections.Generic;
    using Octokit;

    public abstract class Validator
    {
        public abstract IEnumerable<ValidationError> Validate(IEnumerable<Repository> repositories);
    }
}