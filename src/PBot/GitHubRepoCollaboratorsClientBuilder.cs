namespace PBot
{
    using Octokit;
    using Octokit.Internal;

    public static class GitHubRepoCollaboratorsClientBuilder
    {
        public static RepoCollaboratorsClient Build(string accessToken = null)
        {
            var credentialStore = new InMemoryCredentialStore(accessToken == null ? GitHubHelper.Credentials : new Credentials(accessToken));

            var httpClient = new HttpClientAdapter(GitHubHelper.Proxy);

            var connection = new Connection(
                new ProductHeaderValue("PBot"),
                GitHubClient.GitHubApiUrl,
                credentialStore,
                httpClient,
                new SimpleJsonSerializer());

            return new RepoCollaboratorsClient(new ApiConnection(connection));
        }
    }
}