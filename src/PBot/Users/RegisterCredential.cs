namespace PBot.Users
{
    using System.Threading.Tasks;

    public class RegisterCredential : BotCommand
    {
        public RegisterCredential()
            : base("register credential (.*)$",
                "`pbot register credential <credential>=<value>` -  Adds the credential value for the user")
        {
        }

        public override Task Execute(string[] parameters, IResponse response)
        {
            var store = Brain.Get<CredentialStore>() ?? new CredentialStore();

            var input = parameters[1].Split('=');

            store.Add(response.User.Name, input[0],input[1]);

            Brain.Set(store);

            return response.Send("Credentials added");
        }
    }
}
