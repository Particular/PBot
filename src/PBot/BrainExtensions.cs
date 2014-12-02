namespace PBot
{
    using MMBot.Brains;

    public static class BrainExtensions
    {
        public static T Get<T>(this IBrain brain) where T : class
        {
            return brain.Get<T>(typeof(T).FullName).Result;
        }

        public static void Set<T>(this IBrain brain,T data) where T : class
        {
            brain.Set(typeof(T).FullName,data).Wait();
        }
    }
}