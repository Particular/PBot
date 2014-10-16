namespace IssueButler
{
    using System.Collections.Generic;
    using System.Linq;

    class Butler
    {
        public void PerformChores()
        {
            var validationErrors = Validators.SelectMany(v => v.Validate()).ToList();

            //todo: formatters
            var formattedResult = validationErrors.Select(FormatValidationError).ToList();

            Displayers.ForEach(d=>d.Display(formattedResult));
        }

        string FormatValidationError(ValidationError error)
        {
            return string.Format("{0} - {1}", error.Issue.HtmlUrl, error.Reason);
        }

        protected List<Validator> Validators = new List<Validator>();

        protected List<ResultDisplayer> Displayers = new List<ResultDisplayer>();
    }
}