namespace TemeGC
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            Console.WriteLine(@"Hello There!");
        }
    }
}