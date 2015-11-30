namespace PBot
{
    using Octokit;
    using Octokit.Internal;

    public static class GitHubClientBuilder
    {
        public static GitHubClient Build(string accessToken = null)
        {
            var credentialStore = new InMemoryCredentialStore(accessToken == null? GitHubHelper.Credentials : new Credentials(accessToken));

            var httpClient = new HttpClientAdapter(null);

            var connection = new Connection(
                new ProductHeaderValue("PBot"),
                GitHubClient.GitHubApiUrl,
                credentialStore,
                httpClient,
                new SimpleJsonSerializer());

            return new GitHubClient(connection);
        }
    }
}