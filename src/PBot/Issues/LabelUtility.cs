namespace PBot.Issues
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class LabelUtility
    {
        public static async Task<bool> RepositoryHasLabels(RepoInfo repo, params Label[] labels)
        {
            var client = GitHubClientBuilder.Build();
            var existingLabels = await client.Issue.Labels.GetForRepository(repo.Owner, repo.Name);
 
            foreach (var label in labels)
            {
                var foundLabel = existingLabels.SingleOrDefault(l => l.Name.Equals(label.Name, StringComparison.InvariantCulture));

                if (foundLabel == null || foundLabel.Color != label.Color)
                {
                    return false;
                }
            }

            return true;
        }
    }
}