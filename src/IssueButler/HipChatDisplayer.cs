namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using RestSharp;

    public class HipChatDisplayer : ResultDisplayer
    {
        readonly string chatroom;

        readonly string apiKey;
        public HipChatDisplayer(string chatroom)
        {
            this.chatroom = chatroom;

            //api key is in a secured note in LastPass
            apiKey = Environment.GetEnvironmentVariable("HIPCHAT_APITOKEN");

            if (apiKey == null)
            {
                throw new Exception("HipChat api key is needed if the hiphat displayer is to be used");
            }
        }

        public override void Display(IEnumerable<ValidationError> result)
        {

            var maxNumIssuesToShow = 50;
     
            foreach (var repo in result.GroupBy(r => r.Repository.Name))
            {
                if (!repo.Any())
                {
                    continue;
                }

                var sb = new StringBuilder();

                sb.AppendLine("Hello Citizens, please clean up the following issues for " + repo.Key + " <br/>");

                if (repo.Count() > maxNumIssuesToShow)
                {
                    sb.AppendLine(string.Format("Showing top {0} our of {1} invalid issues <br/>",maxNumIssuesToShow, repo.Count()));    
                }


                foreach (var error in repo.Take(maxNumIssuesToShow)) //nsb is to big for now
                {
                    sb.AppendLine(string.Format("<a href=\"{0}\">{1} - {2}<a/><br/>", error.Issue.HtmlUrl, error.Issue.Number, error.Reason));
                }


                SendMessage(sb.ToString());
            }




           
        }

        void SendMessage(string message)
        {
            var client = new RestClient("https://api.hipchat.com/");

            var request = new RestRequest("/v2/room/{name}/notification?auth_token=" + apiKey, Method.POST);


            request.AddUrlSegment("name", chatroom);
            request.RequestFormat = DataFormat.Json;

            request.AddBody(new
            {
                message,
                color = "red"
            });


            // execute the request
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception("Failed to notify on hipchat");
            }
        }
    }
}