namespace PBot
{
    using System.Text;
    using MMBot;
    using Octokit;

    public static class OctoExtensions
    {
        public static string GetExtendedErrorMessage(this ApiValidationException ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }
            if (ex.ApiError == null)
            {
                return ex.Message;
            }
            if (ex.ApiError.Errors == null || ex.ApiError.Errors.Count == 0)
            {
                return ex.Message;
            }

            var sb = new StringBuilder();
            foreach (var error in ex.ApiError.Errors)
            {
                bool codeOrFieldSet = false;
                
                if (!error.Code.IsNullOrEmpty())
                {
                    sb.Append(error.Code);
                    codeOrFieldSet = true;
                }
                
                if (!error.Field.IsNullOrEmpty())
                {
                    if (codeOrFieldSet)
                    {
                        sb.Append(":");
                    }
                    sb.Append(error.Field);
                    codeOrFieldSet = true;
                }

                if (!error.Message.IsNullOrEmpty())
                {
                    if (codeOrFieldSet)
                    {
                        sb.Append(":");
                    }
                    sb.Append(error.Message);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        } 
    }
}