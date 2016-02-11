namespace PBot
{
    using Octokit;

    public static class GitHubRepoCollaboratorsClientBuilder
    {
        public static RepoCollaboratorsClient Build(string accessToken = null)
        {
            var connection = new Connection(new ProductHeaderValue("PBot"));
            connection.Credentials = accessToken == null ? GitHubHelper.Credentials : new Credentials(accessToken);
            return new RepoCollaboratorsClient(new ApiConnection(connection));
        }
    }
}