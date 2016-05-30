namespace PBot.Issues
{
    using Octokit;

    class InvolvedIssue
    {
        public Repository Repo { get; set; }
        public Issue Issue { get; set; }
    }
}
