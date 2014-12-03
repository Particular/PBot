namespace PBot.Users
{
    using System.Collections.Generic;
    using System.Linq;

    public class UserCredentials
    {
        public UserCredentials()
        {
            Credentials = new List<Credential>();
        }

     
        public void AddCredential(string name, string value)
        {
            var existing = Credentials.SingleOrDefault(c => c.Name == name);

            if (existing != null)
            {
                existing.Value = value;
                return;
            }


            Credentials.Add(new Credential
            {
                Name = name,
                Value = value
            });
        }

        public List<Credential> Credentials { get; set; }
        public string Username { get; set; }
    }

    public class Credential
    {
        public string Name { get; set; }

        public string Value { get; set; }

    }
}