namespace PBot
{
    using System.Text.RegularExpressions;
    using Chronic;
    using Nancy;
    using Nancy.Helpers;
    using Nancy.Responses;

    public class GithubSearchModule : NancyModule
    {
        public GithubSearchModule()
        {
            var parser = new Parser();

            // Converts query string expressions from relative datetime to absolute datetime and redirects to github search api
            Get["/search"] = x =>
            {
                var query = HttpUtility.UrlDecode(this.Context.Request.Url.Query);

                var updatedQuery = Regex.Replace(query, @"(\[(.*?)\])",
                    m =>
                    {
                        var originalValue = m.Result("$2");

                        var span = parser.Parse(HttpUtility.UrlDecode(originalValue));

                        var absoluteDate = span?.Start?.Date.ToString("yyyy-MM-dd") ?? originalValue;
                        return absoluteDate;
                    });

                return new RedirectResponse($"https://github.com/search{updatedQuery}", RedirectResponse.RedirectType.Temporary);
            };
        }
    }
}
