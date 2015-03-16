namespace PBot.Octopus
{
    using System;
    using System.Threading.Tasks;
    using OctopusProjectUpdater;

    public class UpdateProjectGroup : BotCommand
    {
        public UpdateProjectGroup()
            : base(
                "update octopus project group (.*)$",
                "pbot update octopus project group <name of the project group in Octopus> - Updates all Octopus Deploy projects in a given group with the latest template.")
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
                var projectGroup = parameters[1];
                await response.Send(string.Format("Got it! Updating all projects in {0}.", projectGroup));

                var facade = new Facade(apiKey, new CachingTemplateRepository(new TeamCityArtifactTemplateRepository()));

                facade.UpdateAllProjects(projectGroup);

                await response.Send("Done!");
            }
        }
    }
}