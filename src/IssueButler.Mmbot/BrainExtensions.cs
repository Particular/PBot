namespace IssueButler.Mmbot
{
    using MMBot.Brains;

    public static class BrainExtensions
    {
        public static T Get<T>(this IBrain brain) where T : class
        {
            return brain.Get<T>(typeof(T).FullName).Result;
        }
    }
}