namespace PBot
{
    using Octokit;

    public static class GitHubClientBuilder
    {
        public static GitHubClient Build(string accessToken = null)
        {
            var client = new GitHubClient(new ProductHeaderValue("PBot"));
            client.Credentials = accessToken == null ? GitHubHelper.Credentials : new Credentials(accessToken);
            return client;
        }
    }
}