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
                "pbot create octopus project <name of the project in Octopus> for project <name of the project in Team City or repo in GitHub> in group <name of group in Octopus> - Creates an Octopus Deploy project for a given repo/TeamCity project.")
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
                await response.Send(string.Format("Got it! Creating project {0} for repo {1} in group {2}.", octopusProject, canonicalProject, group));

                var facade = new Facade(apiKey, new TeamCityArtifactTemplateRepository());

                var url = facade.CreateProject(canonicalProject, group, octopusProject);

                await response.Send(string.Format("Done! Check out the new project here {0}", url));
            }
        }
    }
}