namespace PBot.Octopus
{
    using System;
    using System.Threading.Tasks;
    using OctopusProjectUpdater;

    public class CreateProject : BotCommand
    {
        public CreateProject()
            : base(
                "create octopus project (.*) for repo (.*) in group (.*)$",
                "`pbot create octopus project <name of the project in Octopus> for repo <name of the project in Team City or repo in GitHub> in group <name of group in Octopus>` - Creates an Octopus Deploy project for a given repo/TeamCity project.")
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
                var octopusProject = parameters[1];
                var canonicalProject = parameters[2];
                var group = parameters[3];
                await response.Send($"Got it! Creating project {octopusProject} for repo {canonicalProject} in group {@group}.");

                var facade = new Facade(apiKey, new TeamCityArtifactTemplateRepository());

                var url = facade.CreateProject(canonicalProject, group, octopusProject);

                await response.Send($"Done! Check out the new project here {url}");
            }
        }
    }
}
