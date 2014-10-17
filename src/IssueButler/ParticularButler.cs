namespace IssueButler
{
    class ParticularButler:Butler
    {
        public ParticularButler()
        {
            Validators.Add(new ValidateRepositories("Particular"));
            Displayers.Add(new ConsoleDisplayer());
            Displayers.Add(new HipChatDisplayer("DONOTUSE-FOR-TECH_TESTING"));
        }
    }
}