namespace IssueButler
{
    using System;
    using System.Linq;
    using Octokit;

    public class ValidateRepositories : Chore
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
                    if (issue.Milestone != null)
                    {
                        continue;
                    }



                    var classificationLabels = issue.Labels.Where(l =>ClassificationLabels.Contains(l.Name)).ToList();

                    if (!classificationLabels.Any())
                    {
                        validationErrors.Add(new ValidationError
                        {
                            Reason = "Mandatory classification label is missing, please add one of: " + string.Join(":",ClassificationLabels.All.Select(l=>l.Name)),
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

                var newErrors = validationErrors.Count() - currentNumErrors;

                Console.Out.WriteLine("Validated  {0} - {1}", repository.Name, newErrors == 0 ? "no errors" : newErrors + " errors found");
         
            }
        }


    }
}