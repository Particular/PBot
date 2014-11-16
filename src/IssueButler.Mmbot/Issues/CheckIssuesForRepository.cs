namespace IssueButler.Mmbot.Issues
{
    using System.Linq;
    using Octokit;

    public class CheckIssuesForRepository
    {
        readonly Repository repository;
        readonly GitHubClient client;

        public CheckIssuesForRepository(Repository repository, GitHubClient client)
        {
            this.repository = repository;
            this.client = client;
        }

        public ValidationErrors Execute()
        {
            var issues = client.Issue.GetForRepository(repository.Owner.Login, repository.Name, new RepositoryIssueRequest { State = ItemState.Open }).Result;
            var validationErrors = new ValidationErrors();

            foreach (var issue in issues.Where(i => i.State == ItemState.Open))
            {
                if (issue.PullRequest != null)
                {
                    continue;
                }

                ValidateClassificationLabels(issue, validationErrors, repository);

                if (issue.Milestone != null)
                {
                    continue;
                }

                ValidateNeedsLabels(issue, validationErrors, repository);

            }

            return validationErrors;
        }

        void ValidateNeedsLabels(Issue issue, ValidationErrors validationErrors, Repository repository)
        {
            //only applies to Bugs for now
            if (issue.Labels.All(l => l.Name != "Bug"))
            {
                return;
            }

            var needsLabels = issue.Labels.Where(l => NeedsLabels.Contains(l.Name)).ToList();

            if (!needsLabels.Any())
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Needs: X labels are mandatory for bugs with no milestone, please add one of: " + string.Join(":", NeedsLabels.All.Select(l => l.Name)),
                    Issue = issue,
                    Repository = repository
                });
            }

            if (needsLabels.Count > 1)
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Needs labels are exclusive, please make sure only one of the following exists: " + string.Join(":", NeedsLabels.All.Select(l => l.Name)),
                    Issue = issue,
                    Repository = repository
                });
            }

            //todo: check for activity 
        }

        static void ValidateClassificationLabels(Issue issue, ValidationErrors validationErrors, Repository repository)
        {
            var classificationLabels = issue.Labels.Where(l => ClassificationLabels.Contains(l.Name)).ToList();

            if (!classificationLabels.Any())
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Mandatory classification label is missing, please add one of: " + string.Join(":", ClassificationLabels.All.Select(l => l.Name)),
                    Issue = issue,
                    Repository = repository
                });
            }

            if (classificationLabels.Count > 1)
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Classification labels are exclusive, please make sure only one of the following exists: " + string.Join(":", ClassificationLabels.All.Select(l => l.Name)),
                    Issue = issue,
                    Repository = repository
                });
            }
        }

    }
}