namespace IssueButler
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {

            var butler = new ParticularButler();

            butler.PerformChores();

            Console.Out.WriteLine("Done");

            Console.ReadKey();
        }
    }
}
