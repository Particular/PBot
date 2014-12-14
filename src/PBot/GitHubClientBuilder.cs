namespace PBot
{
    using Octokit;
    using Octokit.Internal;

    public static class GitHubClientBuilder
    {
        public static GitHubClient Build(string accessToken = null)
        {

            var credentialStore = new InMemoryCredentialStore(accessToken == null? GitHubHelper.Credentials : new Credentials(accessToken));

            var httpClient = new HttpClientAdapter(GitHubHelper.Proxy);

            var connection = new Connection(
                new ProductHeaderValue("IssueButler"),
                GitHubClient.GitHubApiUrl,
                credentialStore,
                httpClient,
                new SimpleJsonSerializer());

            return new GitHubClient(connection);
        }
    }
}