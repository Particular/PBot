namespace IssueButler
{
    class ParticularButler:Butler
    {
        public ParticularButler(): base("Particular")
        {
            Validators.Add(new EnsureLabelsExists());
            Validators.Add(new ValidateRepositories());
            Displayers.Add(new ConsoleDisplayer());
            Displayers.Add(new HipChatDisplayer("DONOTUSE-FOR-TECH_TESTING"));
        }
    }
}