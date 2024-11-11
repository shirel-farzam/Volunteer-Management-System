using System;
namespace stage0
{
    partial class program
    {
        static void Main(string[] args)
        {
            welcome4642();
            welcome7701();
            Console.ReadKey();
        }

        static partial void welcome7701();
        private static void welcome4642()
        {
            Console.WriteLine("Enter your name : ");
            string? name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console", name);

        }
    }
}