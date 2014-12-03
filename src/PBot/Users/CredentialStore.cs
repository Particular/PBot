namespace PBot.Users
{
    using System.Collections.Generic;
    using System.Linq;

    public class CredentialStore : List<UserCredentials>
    {
        public void Add(string username,string credential, string value)
        {
            UserCredentials credentials;

            if (!TryGetValue(username, out credentials))
            {
                credentials = new UserCredentials{Username = username};
                Add(credentials);
            }

            credentials.AddCredential(credential,value);
        }

        public bool TryGetValue(string username, out UserCredentials credentials)
        {
            credentials = this.SingleOrDefault(c => c.Username == username);

            return credentials != null;
        }
    }
}