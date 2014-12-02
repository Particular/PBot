namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Repositories;

    [TestFixture]
    public class RemoveRepositoryTests : BotCommandFixture<RemoveRepository>
    {
        [Test]
        public void RemoveValidRepo()
        {
         
            var repoName = "nservicebus";

            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName
                }
            };
            brain.Set(repos);


            Execute("remove", repoName);

            Assert.Null(brain.Get<AvailableRepositories>().SingleOrDefault(r => r.Name == repoName));
        }
      

        [Test]
        public void RemoveValidButNonExistingRepo()
        {

            var repoName = "nservicebus";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            Execute("remove", repoName);

            Assert.NotNull(Messages.Single(m => m.Contains("doesn't exist")));
        }

            

    }
}