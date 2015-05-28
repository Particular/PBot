namespace PBot.Requirements
{
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class CreateRequirement : BotCommand
    {
        string owner;
        string repoName;

        public CreateRequirement()
            : this("Particular", "Requirements")
        {
         
        }

        public CreateRequirement(string owner, string repoName):base("create requirement (.*)$",
            "pbot create requirement <Title> - Opens up a new requirement issue with the specified title")
        {
            this.owner = owner;
            this.repoName = repoName;
        }


        public override async Task Execute(string[] parameters, IResponse response)
        {
            if (parameters.Length < 2)
            {
                await response.Send(string.Format("I don't understand '{0}'.", string.Join(" ", parameters.Select(x => "["+x+"]"))));
                return;
            }
            var title = parameters[1];
           
            string accessToken;

            if (!TryGetCredential("github-accesstoken", out accessToken))
            {
                await response.Send("I couldn\'t find a github access token in your credentials. You need to add one so that I can create the requirement on your behalf. Does it sound good? Here are the instructions https://github.com/Particular/Housekeeping/wiki/Generate-GitHub-access-token-for-PBot");

                return;
            }

            var client = GitHubClientBuilder.Build(accessToken);

            
            
            var issue = await client.Issue.Create(owner, repoName, new NewIssue(title)
            {
                Body = bodyTemplate
            });

            await response.Send("Requirement created " + issue.HtmlUrl);
        }

        static string bodyTemplate =
@"<!---
Please see https://github.com/Particular/Strategy/issues/5 for a detailed explanation of each section.
-->
### Customer communication
<!---
* What would we tell existing customers about this feature/concern?
* How would we describe this impact in a way that our end users would understand?
-->

### Impact assessment
<!--
Make sure to document both negative and positive impact on the relevant user roles (see below).
After you've done this you should be able to label this issue with one of the `Impact: X` labels
-->
#### How many customers will be impacted?
<!--
Try to reason about what user segment this one impacts. For example features releated to sagas would only affect customers that are actually using sagas
-->
#### Impact on roles
<!--
How does this impact our the different roles in our industry? 
What use cases does it unlock/ block?

Only list the ones that are impacted

Full list of roles: https://github.com/Particular/Vision/labels/User%20role

-->

### Alignment with vision
<!--
Add links back to items in our vision repo that is the main driver for this specific requirement. 

Eg:

Improves our [Installation Experience](https://github.com/Particular/Vision/issues/13) for the Xyz capability
-->
### Plan of Attack
<!--
What is the high level plan of attack to get this issue done done?
-->

### Stories
<!---

Note: This is section is optional

* What would we tell people that doesn't know about our platform or even messaging that would make them understand why the platform is a good fit for them?

* Is there a more general story that could be told that would help make developers better?
-->
";
    }
}
