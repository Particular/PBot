using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueButler.Mmbot.Repositories;

namespace PBot.Buildserver
{
    using System;

    public class ListCurrentlyFailingBuilds : BotCommand
    {
        public ListCurrentlyFailingBuilds()
            : base("list failing builds for (.*)$", "pbot list failing builds for <buildname|all projects|my repos> - Checks all issues in the specified repository and reports needed actions")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var projectName = parameters[1];

            IEnumerable<Project> projects;
            var client = new TeamCity("http://builds.particular.net");
            var allProjects = client.ListProjects();

            var displaySuccessMessage = true;

            switch (projectName)
            {
                case "all projects":
                    projects = allProjects;
                    displaySuccessMessage = false;
                    break;
                case "my repos":
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
                        return;
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


                sb.AppendLine("*" + project.Key.Name + "*");
                var buildTypes = project.GroupBy(b => b.BuildType);

                foreach (var buildType in buildTypes)
                {

                    sb.AppendLine("* `" + buildType.Key + "`");

                    foreach (var failedBuild in buildType)
                    {
                        sb.AppendLine(string.Format("     - {0}({1}) {2}", failedBuild.Number, failedBuild.Branch, failedBuild.Url));

                    }
                }

                await response.Send(sb.ToString());
            }

            if (projectWithFailures.Count > 0)
            {
                var totalDownTimeT = projectWithFailures.SelectMany(p=>p.Select(bt => DateTime.Now - client.GetBuild(bt.Id).FinishedAt)).ToList();


                var totalDownTime  =  totalDownTimeT.Sum(ts=>ts.TotalDays);

                await response.Send(string.Format("Summary: {0} failed builds, Total down time: {1} days", projectWithFailures.SelectMany(p=>p).Count(), totalDownTime));
            }

            if (projectWithFailures.Count == 0 && displaySuccessMessage)
            {
                await response.Send("All builds green for: " + string.Join(",", projects.Select(p => p.Name)));
            }
        }
    }
}