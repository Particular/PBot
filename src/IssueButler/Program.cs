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

            if (ConsolePresent)
            {
                Console.ReadKey();              
            }
        }

        static bool ConsolePresent
        {
            get
            {
                if (consolePresent == null)
                {
                    consolePresent = true;
// ReSharper disable once UnusedVariable
                    try { var window_height = Console.WindowHeight; }
                    catch { consolePresent = false; }
                }
                return consolePresent.Value;
            }
        }

        static bool? consolePresent;
    }
}
