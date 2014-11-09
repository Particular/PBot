namespace IssueButler.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;

    public class TestBrain : IBrain
    {
        public void Initialize(Robot robot)
        {
            throw new NotImplementedException();
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task<T> Get<T>(string key)
        {
            return Task<T>.Factory.StartNew(()=>(T) values[key]);
        }

        public Task Set<T>(string key, T value)
        {
            return Task.Factory.StartNew(() => values[key] = value);
        }

        Dictionary<string,object> values = new Dictionary<string, object>(); 

        public Task Remove<T>(string key)
        {
            throw new NotImplementedException();
        }
    }
}