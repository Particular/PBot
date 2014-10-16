namespace IssueButler
{
    class ParticularButler:Butler
    {
        public ParticularButler()
        {
            Validators.Add(new ValidateRepositories("Particular"));
            Displayers.Add(new ConsoleDisplayer());
        }
    }
}