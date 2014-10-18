namespace IssueButler.Chores
{
    public abstract class Chore
    {
        public  abstract void PerformChore(Brain brain);

        public virtual string Description
        {
            get { return GetType().FullName; }
        }
    }
}