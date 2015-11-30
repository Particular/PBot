﻿using System;
using System.Diagnostics;
using Octokit;


//copy pasted from Octokit.net
public static class Helper
{
    static readonly Lazy<Credentials> _credentialsThunk = new Lazy<Credentials>(() =>
    {
        var githubUsername = Environment.GetEnvironmentVariable("PBOT_GITHUBUSERNAME");
        UserName = githubUsername;

        var githubToken = Environment.GetEnvironmentVariable("PBOT_OAUTHTOKEN");

        if (githubToken != null)
            return new Credentials(githubToken);

        var githubPassword = Environment.GetEnvironmentVariable("PBOT_GITHUBPASSWORD");

        if (githubUsername == null || githubPassword == null)
            return null;

        return new Credentials(githubUsername, githubPassword);
    });

    static Helper()
    {
        // Force reading of environment variables.
        // This wasn't happening if UserName/Organization were 
        // retrieved before Credentials.
        Debug.WriteIf(Credentials == null, "No credentials specified.");
    }

    public static string UserName { get; private set; }

    public static Credentials Credentials => _credentialsThunk.Value;

    //todo: do we create a ParticularTest org that's paid?
    public static bool IsPaidAccount => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("PBOT_PRIVATEREPOSITORIES"));

    public static void DeleteRepo(Repository repository)
    {
        if (repository != null)
            DeleteRepo(repository.Owner.Login, repository.Name);
    }

    public static void DeleteRepo(string owner, string name)
    {
        var api = GetAuthenticatedClient();
        try
        {
            api.Repository.Delete(owner, name).Wait(TimeSpan.FromSeconds(15));
        }
        catch(Exception ex)
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
        return new GitHubClient(new ProductHeaderValue("PBotTests"))
        {
            Credentials = Credentials
        };
    }
}