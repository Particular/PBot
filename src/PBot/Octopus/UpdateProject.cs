namespace PBot.Octopus
{
    using System;
    using System.Threading.Tasks;
    using OctopusProjectUpdater;

    public class UpdateProject : BotCommand
    {
        public UpdateProject()
            : base(
                "update octopus project (.*)$",
                "`pbot update octopus project <name of the project in Octopus>` - Updates an Octopus Deploy project with the latest template for its group.")
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
                await response.Send(string.Format("Got it! Updating project {0}.", octopusProject));

                var facade = new Facade(apiKey, new TeamCityArtifactTemplateRepository());

                var url = facade.UpdateProject(octopusProject);

                await response.Send(string.Format("Done! Check out the updated project here {0}", url));
            }
        }
    }
}
