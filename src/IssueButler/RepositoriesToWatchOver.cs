namespace IssueButler
{
    using System.Collections;
    using System.Collections.Generic;
    using Octokit;

    public class RepositoriesToWatchOver:IEnumerable<Repository>
    {
        readonly IEnumerable<Repository> repositories;

        public RepositoriesToWatchOver(IEnumerable<Repository> repositories)
        {
            this.repositories = repositories;
        }

        public IEnumerator<Repository> GetEnumerator()
        {
            return repositories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}