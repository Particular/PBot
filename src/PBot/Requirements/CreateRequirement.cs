namespace PBot.Requirements
{
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class CreateRequirement : BotCommand
    {
        readonly string owner;
        readonly string repoName;

        public CreateRequirement(string owner = "Particular", string repoName = "Requirements")
            : base("create requirement (.*)$",
            "pbot create requirement \"<Title>\" - Opens up a new requirement issue with the specified title")
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
### Stories
<!---
* What would we tell people that doesn't know about our platform or even messaging that would make them understand why the platform is a good fit for them?

* Is there a more general story that could be told that would help make developers better?
-->
### Impact assessment
<!--
Make sure to document both negative and positive impact on the relevant personas.
After you've done this you should be able to label this issue with one of the `Impact: X` labels
-->
#### How many customers will be impacted?
#### Impact on personas
<!--
How does this impact our personas? 
What use cases does it unlock/ block?

Only list the ones that are impacted

Personas (https://github.com/Particular/Vision/labels/Persona): 

Developer Dave,Tech. Lead Tom,Architect Archie, Ops person Opie, DevOps person Deva,Manager,Operations (Particular), Sales (Particular), Engineering (Particular), Marketing (Particular), Guidance (Particular), Support (Particular), Product (Particular), HR (Particular)
-->

### Alignment with vision
<!--
Add links back to items in our vision repo that is the main driver for this specific requirement. 

Eg:

Improves our [Installation Experience](https://github.com/Particular/Vision/issues/13)
-->
### Plan of Attack
<!--
What is the high level plan of attack to get this issue done done?
-->
";
    }
}