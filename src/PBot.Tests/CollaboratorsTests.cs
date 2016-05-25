namespace PBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using PBot;

    [TestFixture]
    public class CollaboratorsTests
    {
        [Test, Explicit("Does the actual work for now")]
        [TestCase("Particular", new[] { "aaa", "bbb" })]
        public async Task DeleteCollaborators(string organization, string[] collaborators)
        {
            var client = GitHubClientBuilder.Build();
            var repositories = await client.Repository.GetAllForOrg(organization);
            var removals = repositories.Select(async repository =>
            {
                await DeleteCollaborators(client, repository, collaborators);
            }).ToList();

            await Task.WhenAll(removals);
            Console.Out.WriteLine(
                $"Deleted {collaborators.Length:N0} collaborators from {removals.Count:N0} repositories.");
        }

        static async Task DeleteCollaborators(
            IGitHubClient client, Repository repository, IEnumerable<string> collaborators)
        {
            var removals = collaborators.Select(async collaborator =>
            {
                try
                {
                    await client.Repository.Collaborator.Delete(repository.Owner.Login, repository.Name, collaborator);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(
                        $"Failed to delete collaborator '{collaborator}' from '{repository.FullName}'. {ex.Message}");
                }
            });

            await Task.WhenAll(removals);
        }
    }
}