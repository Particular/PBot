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
                "pbot create TC project for <name of the repo> in <id of parent project> - Creates a TeamCity project for a given repo inside a parent project.")
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
                await response.Send(string.Format("Got it! Creating a TeamCity project for repo {0} in {1} parent project.", repo, parentId));

                var facade = new Facade(apiKey, new TeamCityArtifactTemplateRepository());

                var url = facade.StartScriptTask(string.Format(
                    @"C:\ProgramData\JetBrains\TeamCity\plugins\.tools\TeamCityProjectCreator\tools\CreateProject.ps1 -ProjectName {0} -ParentId {1}", repo, parentId),
                    new[]
                    {
                        "machines-65" //TeamCity server
                    });

                await response.Send(string.Format("Process started! Check out the result here {0}", url));
            }
        }
    }
}