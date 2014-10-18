namespace IssueButler
{
    using System;
    using System.Collections.Generic;
    using IssueButler.Chores;

    class Butler
    {
        protected Brain Brain = new Brain();

      
        public void PerformChores()
        {

            Chores.ForEach(c =>
            {
                Console.Out.WriteLine("Performing " + c.Description);
                c.PerformChore(Brain);
                Console.Out.WriteLine("Completed " + c.Description);
                
            });

            Displayers.ForEach(d => d.Display(Brain.Recall<ValidationErrors>()));
        }

   
        protected List<Chore> Chores = new List<Chore>();

        protected List<ResultDisplayer> Displayers = new List<ResultDisplayer>();
    }
}