namespace PBot.Octopus
{
    using System;
    using System.Threading.Tasks;
    using OctopusProjectUpdater;

    public class CreateTeamCityProject : BotCommand
    {
        public CreateTeamCityProject()
            : base(
                "create TC project for (.*) in (.*)$",
                "`pbot create TC project for <name of the repo> in <id of parent project>` - Creates a TeamCity project for a given repo inside a parent project.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var apiKey = Environment.GetEnvironmentVariable("OCTOPUS_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await response.Send("Octopus Deploy API key has not been configured. Please set it as environment variable `OCTOPUS_API_KEY`");
            }
            else
            {
                var repo = parameters[1];
                var parentId = parameters[2];
                await response.Send($"Got it! Creating a TeamCity project for repo {repo} in {parentId} parent project.");

                var facade = new Facade(apiKey, new TeamCityArtifactTemplateRepository());

                var url = facade.StartScriptTask($@"C:\ProgramData\JetBrains\TeamCity\plugins\.tools\TeamCityProjectCreator\tools\CreateProject.ps1 -ProjectName {repo} -ParentId {parentId}",
                    new[]
                    {
                        "machines-65" //TeamCity server
                    });

                await response.Send($"Process started! Check out the result here {url} or go straight to the project settings {Constants.BuildServerRoot}admin/editProject.html?projectId={parentId}_{repo}");
            }
        }
    }
}