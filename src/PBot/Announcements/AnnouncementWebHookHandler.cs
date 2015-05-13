namespace PBot.Announcements
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using Nancy;
    using Nancy.ModelBinding;
    using HttpStatusCode = Nancy.HttpStatusCode;

    public class AnnouncementWebHookHandler : NancyModule
    {
        public AnnouncementWebHookHandler()
        {
            Post["/webhooks/issues", true] = async (x, ct) =>
            {
                try
                {
                    using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes("Secret")))
                    {
                        var hashValue = hmac.ComputeHash(Request.Body);
                        var hashString = "sha1=" + Encoding.UTF8.GetString(hashValue);
                        var receivedHash = Request.Headers["X-Hub-Signature"].FirstOrDefault();

                        if (hashString != receivedHash)
                        {
                            return string.Format("Received signature: {0}, Calculated signature: {1}", receivedHash, hashString);
                        }
                    }

                    var issueEvent = this.Bind<IssueEvent>();
                    var client = GitHubClientBuilder.Build();
                    var issue = await client.Issue.Get(issueEvent.Repository.Owner.Login, issueEvent.Repository.Name, issueEvent.Issue.Number);
                    var body = issue.Body;

                    var smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential("szymon.pobiega@nservicebus.com", "hkkv7ky2yc")
                    };
                    await smtp.SendMailAsync("szymon.pobiega@particular.net", "szymon.pobiega@gmail.com", "Test", body);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                return HttpStatusCode.OK;
            };
        }
    }

    public class IssueEvent
    {
        public Action Action { get; set; }
        public Issue Issue { get; set; }
        public Repository Repository { get; set; }
    }

    public class Repository
    {
        public string Name { get; set; }
        public Owner Owner { get; set; }
    }

    public class Owner
    {
        public string Login { get; set; }
    }

    public class Issue
    {
        public string Url { get; set; }
        public int Number { get; set; }
    }

    public enum Action
    {
        Assigned,
        Unassigned,
        Labeled,
        Unlabeled,
        Opened,
        Closed,
        Reopened
    }
}