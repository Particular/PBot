namespace PBot.SyncOMatic
{
    using System.Collections.Generic;
    using global::SyncOMatic;

    public static class DefaultTemplateRepo
    {
        static List<SyncItem> ItemsToSync = new List<SyncItem>();

        static DefaultTemplateRepo()
        {
            ItemsToSync.Add(new SyncItem
            {
                Parts = new Parts("Particular/RepoStandards", TreeEntryTargetType.Blob, "master", ".gitignore")
            });

            ItemsToSync.Add(new SyncItem
            {
                Parts = new Parts("Particular/RepoStandards", TreeEntryTargetType.Blob, "master", ".gitattributes")
            });

            ItemsToSync.Add(new SyncItem
            {
                Parts = new Parts("Particular/RepoStandards", TreeEntryTargetType.Blob, "master", "src/nuget.config")
            });

            ItemsToSync.Add(new SyncItem
            {
                Target = "{{src.root}}/{{solution.name}}.sln.DotSettings",
                Parts = new Parts("Particular/RepoStandards", TreeEntryTargetType.Blob, "master", "src/RepoName.sln.DotSettings")
            });
        }

        public static List<SyncItem> WithLicense(string license)
        {
            if (license == null)
            {
                return ItemsToSync;
            }

            var items = new List<SyncItem>(ItemsToSync);
            var licenseFile = license != "standard" ? $"{license.ToUpper()}.md" : "RPL.md";
            items.Add(new SyncItem
            {
                Target = "LICENSE",
                Parts = new Parts("Particular/RepoStandards", TreeEntryTargetType.Blob, "master", licenseFile)
            });
            return items;
        }
    }
}
