namespace IssueButler
{
    using Octokit;

    public class ValidationError
    {
        public string Reason;
        public Issue Issue;
        public Repository Repository;
    }
}