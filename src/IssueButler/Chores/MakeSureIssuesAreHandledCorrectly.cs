namespace IssueButler.Chores
{
    using System;
    using System.Linq;
    using Octokit;

    public class MakeSureIssuesAreHandledCorrectly : Chore
    {

        public override void PerformChore(Brain brain)
        {
            var validationErrors = brain.Recall<ValidationErrors>();

            var repositories = brain.Recall<RepositoriesToWatchOver>();

            var client = brain.Recall<GitHubClient>();



            foreach (var repository in repositories)
            {
                var currentNumErrors = validationErrors.Count();

                var issues = client.Issue.GetForRepository(repository.Owner.Login, repository.Name,new RepositoryIssueRequest{State = ItemState.Open}).Result;

                foreach (var issue in issues.Where(i=>i.State == ItemState.Open))
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

                var newErrors = validationErrors.Count() - currentNumErrors;

                Console.Out.WriteLine("Validated  {0} - {1}", repository.Name, newErrors == 0 ? "no errors" : newErrors + " errors found");
         
            }
        }

        void ValidateNeedsLabels(Issue issue, ValidationErrors validationErrors, Repository repository)
        {
            var needsLabels = issue.Labels.Where(l => NeedsLabels.Contains(l.Name)).ToList();

            if (!needsLabels.Any())
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Needs: X labels are mandatory for issues with no milestone, please add one of: " + string.Join(":", ClassificationLabels.All.Select(l => l.Name)),
                    Issue = issue,
                    Repository = repository
                });
            }

            if (needsLabels.Count > 1)
            {
                validationErrors.Add(new ValidationError
                {
                    Reason = "Needs labels are exclusive, please make sure only one of the following exists: " + string.Join(":", ClassificationLabels.All.Select(l => l.Name)),
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