using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueButler.Mmbot.Repositories;

namespace PBot.Buildserver
{
    public class ListCurrentlyFailingBuilds : BotCommand
    {
        public ListCurrentlyFailingBuilds()
            : base("list failing builds for (.*)$", "pbot list failing builds for <buildname|all builds|my builds> - Checks all issues in the specified repository and reports needed actions")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var projectName = parameters[1];

            IEnumerable<Project> projects;
            var client = new TeamCity("http://builds.particular.net");
            var allProjects = client.ListProjects();

            switch (projectName)
            {
                case "all builds":
                    projects = allProjects;
                    break;
                case "my builds":
                    var username = response.User.Name;

                    var repoNames = Brain.Get<AvailableRepositories>()
                        .Where(r => r.Caretaker == username)
                        .Select(r => r.Name)
                        .ToList();

                    projects = allProjects.Where(p => repoNames.Any(r => r == p.Name));
                    break;

                default:
                    var project = allProjects.FirstOrDefault(p => p.Name == projectName);

                    if (project == null)
                    {
                        await response.Send(string.Format("No build with name `{0}` could be found on the buildserver", projectName));
                    }
                    projects = new[]
                    {
                        project
                    };
                    break;
            }


            var projectWithFailures = projects.SelectMany(p => client.ListCurrentBuilds(p.Id, new[]
            {
                "master",
                "develop"
            }))
                .Where(b => b.Status == BuildStatus.Failed)
                .GroupBy(b => b.Project)
                .ToList();

            foreach (var project in projectWithFailures)
            {
                var sb = new StringBuilder();


                sb.AppendLine("### " + project.Key.Name);
                var buildTypes = project.GroupBy(b => b.BuildType);

                foreach (var buildType in buildTypes)
                {

                    sb.AppendLine("* `" + buildType.Key + "`");

                    foreach (var failedBuild in buildType)
                    {
                        sb.AppendLine(string.Format("     - [{0} - {1}]({2})", failedBuild.Number, failedBuild.Branch, failedBuild.Url));

                    }
                }

                await response.Send(sb.ToString());
            }
        }
    }
}