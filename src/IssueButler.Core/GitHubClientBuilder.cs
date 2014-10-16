namespace ReleaseStats.Providers.GitHub
{
    using Octokit;
    using Octokit.Internal;

    static class GitHubClientBuilder
    {
        public static GitHubClient Build()
        {
            var credentialStore = new InMemoryCredentialStore(GitHubHelper.Credentials);

            var httpClient = new HttpClientAdapter(GitHubHelper.Proxy);

            var connection = new Connection(
                new ProductHeaderValue("Issue"),
                GitHubClient.GitHubApiUrl,
                credentialStore,
                httpClient,
                new SimpleJsonSerializer());

            return new GitHubClient(connection);
        }
    }
}