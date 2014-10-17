namespace IssueButler
{
    using System.Collections.Generic;
    using System.Linq;

    class Butler
    {
        public void PerformChores()
        {
            var validationErrors = Validators.SelectMany(v => v.Validate()).ToList();

            Displayers.ForEach(d => d.Display(validationErrors));
        }

        protected List<Validator> Validators = new List<Validator>();

        protected List<ResultDisplayer> Displayers = new List<ResultDisplayer>();
    }
}