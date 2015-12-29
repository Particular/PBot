namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    public class Release_statistics
    {
        [Test, Explicit]
        public async void AllReleases()
        {
            Console.Out.WriteLine("### All Releases");

            var client = GitHubClientBuilder.Build();

            var organization = "Particular";

            var repositories = await client.Repository.GetAllForOrg(organization);

            foreach (var repo in repositories.Where(r => !r.Private && !r.Description.Contains("[State: Deprecated]")))
            {
                var releases = await client.Release.GetAll(organization, repo.Name);

                foreach (var release in releases.Where(r => !r.Prerelease && !r.Draft))
                {
                    var tagParts = release.TagName.Split('.');

                    if (tagParts.Length < 3)
                    {
                        throw new Exception($"Bad tagname {release.TagName} for release {release.HtmlUrl}");
                    }

                    var isMajorOrMinor = tagParts[2] == "0";
                    Console.Out.WriteLine(string.Join("\t", new List<string>
                    {
                        repo.Name,
                        release.TagName,
                        release.PublishedAt.Value.ToString("d"),
                        (!isMajorOrMinor).ToString(),
                        release.HtmlUrl
                    }));
                }
            }
        }
    }
}