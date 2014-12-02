namespace PBot.Issues
{
    using System;
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
            var lastActivityOnIssue = issue.UpdatedAt; //todo: does this include comments?

            if (issue.Labels.Any(l => l.Name == "Bug") && !issue.Labels.Any(l => l.Name.StartsWith("Needs:")))
            {
                if (lastActivityOnIssue < DateTime.UtcNow.AddDays(-3))
                {
                    validationErrors.Add(new ValidationError
                    {
                        Reason = "This bug doesn't seem to be triaged, use one of the `Needs: X` labels to remember what the next steps are",
                        Issue = issue,
                        Repository = repository
                    });
        
                }
            
                return;
            }

            if (issue.Labels.Any(l =>l.Name== "Needs: Triage") && lastActivityOnIssue < DateTime.UtcNow.AddDays(-3))
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Issue needs triage but hasn't been updated for 3 days",
                    Issue = issue,
                    Repository = repository
                });
            }


            if (issue.Labels.Any(l => l.Name == "Needs: Patch") && lastActivityOnIssue < DateTime.UtcNow.AddDays(-5))
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Issue needs a patch but hasn't been updated for 3 days",
                    Issue = issue,
                    Repository = repository
                });
            }

            if (issue.Labels.Any(l => l.Name == "Needs: Reproduction") && lastActivityOnIssue < DateTime.UtcNow.AddDays(-7))
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Issue needs a repro but hasn't been touched in the last 7 days",
                    Issue = issue,
                    Repository = repository
                });
            }
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