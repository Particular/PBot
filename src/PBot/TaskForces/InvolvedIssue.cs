namespace PBot.TaskForces
{
    using Octokit;

    internal class InvolvedIssue
    {
        public Repository Repo { get; set; }
        public Issue Issue { get; set; }
    }
}
