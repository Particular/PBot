namespace PBot.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using PBot.Issues;

    [TestFixture]
    public class LabelUtilityTest
    {
        [Test]
        public async Task Should_find_label_that_exists()
        {
            const string labelName = "Bug";
            const string labelColor = "eb6420";
            var repoInfo = new RepoInfo { Name = "RepoStandards", Owner = "Particular" };

            var result = await LabelUtility.RepositoryHasLabels(repoInfo, new Label(null, labelName, labelColor));
            Assert.True(result, "Should return true");
        }
        
        [Test]
        public async Task Should_not_find_label_that_does_not_exist()
        {
            const string labelName = "NonExistentLabel";
            const string labelColor = "000000";
            var repoInfo = new RepoInfo { Name = "RepoStandards", Owner = "Particular" };

            var result = await LabelUtility.RepositoryHasLabels(repoInfo, new Label(null, labelName, labelColor));
            Assert.False(result, "Should return false");
        }

        [Test]
        public async Task Should_find_multiple_labels()
        {
            var label1 = new Label(null, "Bug", "eb6420");
            var label2 = new Label(null, "Feature", "02e10c");
            
            var repoInfo = new RepoInfo { Name = "RepoStandards", Owner = "Particular" };

            var result = await LabelUtility.RepositoryHasLabels(repoInfo, label1, label2);
            Assert.True(result, "Should return true");
        }

        [Test]
        public async Task Should_not_find_labels_if_one_of_them_does_not_exist()
        {
            var label1 = new Label(null, "Bug", "eb6420");
            var label2 = new Label(null, "NonExistentLabel", "000000");
            
            var repoInfo = new RepoInfo { Name = "RepoStandards", Owner = "Particular" };

            var result = await LabelUtility.RepositoryHasLabels(repoInfo, label1, label2);
            Assert.False(result, "Should return false");
        }
    }
}