namespace IssueButler.Mmbot.Repositories
{
    using System.Collections.Generic;

    public class AvailableRepositories:List<AvailableRepositories.Repository>
    {
        public class Repository
        {
            public string Name { get; set; }
            public string Caretaker { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}