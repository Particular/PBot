namespace PBot.Octopus
{
    using System.Threading.Tasks;
    using OctopusProjectUpdater;

    public class UpdateProject : BotCommand
    {
        public UpdateProject()
            : base(
                "update octopus project (.*)$",
                "pbot update octopus project <name of the project in Octopus> - Updates an Octopus Deploy project with the latest template for its group.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var octopusProject = parameters[1];
            await response.Send(string.Format("Got it! Updating project {0}.", octopusProject));

            var facade = new Facade("", new TeamCityArtifactTemplateRepository());

            facade.UpdateProject(octopusProject);

            await response.Send("Done!");
        }
    }
}