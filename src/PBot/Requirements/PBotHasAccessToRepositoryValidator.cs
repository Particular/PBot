﻿namespace PBot.Requirements
{
    using System.Threading.Tasks;
    using Octokit;

    public class PBotHasAccessToRepositoryValidator
    {
        readonly IRepoCollaboratorsClient client;
        readonly string repositoryName;

        public PBotHasAccessToRepositoryValidator(IRepoCollaboratorsClient client, string repositoryName)
        {
            this.client = client;
            this.repositoryName = repositoryName;
        }

        public Task<bool> Perform()
        {
            return client.IsCollaborator("Particular", repositoryName, "particularbot");
        }
    }
}