partial class Program
{
    private static void Main(string[] args)
    {
        welcome4642();
        Console.ReadKey();
    }
    static partial void welcome7701();
    private static void welcome4642()
    {
        Console.WriteLine("ENTER YOUR NAME: ");
        string name = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console aplication", name);
    }
}