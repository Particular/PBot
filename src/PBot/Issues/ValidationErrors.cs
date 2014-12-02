namespace PBot.Issues
{
    using System.Collections;
    using System.Collections.Generic;

    public class ValidationErrors:IEnumerable<ValidationError>
    {
        public IEnumerator<ValidationError> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        List<ValidationError> errors = new List<ValidationError>();

        public void Add(ValidationError validationError)
        {
            errors.Add(validationError);
        }
    }
}