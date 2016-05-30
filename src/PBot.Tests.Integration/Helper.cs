using System;
using System.Diagnostics;
using Octokit;

//copy pasted from Octokit.net
public static class Helper
{
    static Lazy<Credentials> credentialsThunk = new Lazy<Credentials>(() =>
    {
        var githubToken = Environment.GetEnvironmentVariable("PBOT_OAUTHTOKEN");
        if (githubToken != null)
        {
            return new Credentials(githubToken);
        }

        var githubUsername = Environment.GetEnvironmentVariable("PBOT_GITHUBUSERNAME");
        var githubPassword = Environment.GetEnvironmentVariable("PBOT_GITHUBPASSWORD");
        if (githubUsername != null && githubPassword != null)
        {
            return new Credentials(githubUsername, githubPassword);
        }

        // If there's no specific pbot variables, look for octokit ones

        githubToken = Environment.GetEnvironmentVariable("OCTOKIT_OAUTHTOKEN");
        if (githubToken != null)
        {
            return new Credentials(githubToken);
        }

        githubUsername = Environment.GetEnvironmentVariable("OCTOKIT_GITHUBUSERNAME");
        githubPassword = Environment.GetEnvironmentVariable("OCTOKIT_GITHUBPASSWORD");
        if (githubUsername != null && githubPassword != null)
        {
            return new Credentials(githubUsername, githubPassword);
        }

        return null;
    });

    static Helper()
    {
        // Force reading of environment variables.
        // This wasn't happening if UserName/Organization were
        // retrieved before Credentials.
        Debug.WriteIf(Credentials == null, "No credentials specified.");
    }

    static Credentials Credentials => credentialsThunk.Value;

    public static void DeleteRepo(Repository repository)
    {
        if (repository != null)
        {
            DeleteRepo(repository.Owner.Login, repository.Name);
        }
    }

    public static void DeleteRepo(string owner, string name)
    {
        var api = GetAuthenticatedClient();
        try
        {
            api.Repository.Delete(owner, name).Wait(TimeSpan.FromSeconds(15));
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine(ex);
        }
    }

    public static string MakeNameWithTimestamp(string name)
    {
        return string.Concat(name, "-", DateTime.UtcNow.ToString("yyyyMMddhhmmssfff"));
    }

    public static IGitHubClient GetAuthenticatedClient()
    {
        var client = new GitHubClient(new ProductHeaderValue("PBotTests"));
        if (Credentials != null)
            client.Credentials = Credentials;
        return client;
    }
}