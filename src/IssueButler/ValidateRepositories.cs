namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Octokit;

    public class ValidateRepositories : Validator
    {
        readonly string organization;
        GitHubClient client;
        public ValidateRepositories(string organization)
        {
            this.organization = organization;
            client = GitHubClientBuilder.Build();
        }

        public override IEnumerable<ValidationError> Validate()
        {
            var repos = client.Repository.GetAllForOrg(organization).Result
                .Where(r =>r.HasIssues && !r.Private  && r.Name.StartsWith("NServiceBus") || r.Name.StartsWith("Service"))//for now
                .ToList();

            var validationErrors = new List<ValidationError>();


            foreach (var repository in repos)
            {
                var issues = client.Issue.GetForRepository("Particular", repository.Name,new RepositoryIssueRequest{State = ItemState.Open}).Result;

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
                            Reason = "Mandatory classification label is missing, please add one of: " + string.Join(":",ClassificationLabels.All),
                            Issue = issue,
                            Repository = repository
                        });

                    }

                }
                Console.Out.WriteLine("Completed " + repository.Name);
         
            }
            return validationErrors;

        }

     
    }
}