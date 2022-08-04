using System;

namespace Go
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new UserInterface();
            game.Run();
        }
    }
}
