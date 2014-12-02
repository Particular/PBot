namespace PBot.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;

    public class TestBrain : IBrain
    {
        Dictionary<string, object> values = new Dictionary<string, object>();

        public void Initialize(Robot robot)
        {
        }

        public Task Close()
        {
            return Task.FromResult(0);
        }

        public Task<T> Get<T>(string key)
        {
            return Task.FromResult((T)values[key]);
        }

        public Task Set<T>(string key, T value)
        {
            values[key] = value;
            return Task.FromResult(0);
        }

        public Task Remove<T>(string key)
        {
            values.Remove(key);
            return Task.FromResult(0);
        }
    }
}