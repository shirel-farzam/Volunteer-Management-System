using System;
namespace stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            welcome4642();
            welcome7701();
            Console.ReadKey();
        }

        
        private static void welcome4642()
        {
            Console.Write("Enter your name: ");
            string? name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console", name);
        }
        static partial void welcome7701();
    }
}