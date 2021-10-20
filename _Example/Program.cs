using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotRPG._Example
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new Game1())
                    game.Run();
            }
            catch (Exception e)
            {
                File.WriteAllText("error-" + DateTime.Now + ".txt", e.Message);
            }
        }
    }
}
