namespace PBot.Users
{
    using System.Threading.Tasks;

    public class ListCredentials : BotCommand
    {
        public ListCredentials()
            : base("list credentials",
                "`pbot list credentials` - Lists credentials for the user")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {

            UserCredentials userCredentials;

            if (!Brain.Get<CredentialStore>().TryGetValue(response.User.Name, out userCredentials))
            {
                await response.Send("No credentials found for " + response.User.Name).IgnoreWaitContext();
                return;
            }


            await response.Send("Credentials for " + response.User.Name).IgnoreWaitContext();

            foreach (var credential in userCredentials.Credentials)
            {
                await response.Send(credential.Name + ": " + credential.Value).IgnoreWaitContext();
            }
        }
    }
}
