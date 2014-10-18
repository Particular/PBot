namespace IssueButler
{
    class ParticularButler : Butler
    {
        public ParticularButler()
        {
            Brain.Remember(GitHubClientBuilder.Build());
            Brain.Remember(new ValidationErrors());
            
            
            Chores.Add(new GetRepositoriesToWatch("Particular"));
            Chores.Add(new EnsureLabelsExists());
            Chores.Add(new ValidateRepositories());
            Chores.Add(new DumpErrorsToConsole());
            Chores.Add(new NotifyOnErrorsViaHipChat("DONOTUSE-FOR-TECH_TESTING"));
        }
    }
}