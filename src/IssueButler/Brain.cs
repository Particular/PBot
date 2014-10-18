namespace IssueButler
{
    using System.Collections.Generic;

    public class Brain
    {
        public void Remember<T>(T thingToRemember)
        {
            memory[typeof(T).FullName] = thingToRemember;
        }

        public T Recall<T>()
        {
            return (T) memory[typeof(T).FullName];
        }

        Dictionary<string,object> memory = new Dictionary<string, object>(); 
    }
}