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
#if !DEBUG
            try
            {
#endif
                using (var game = new Game1())
                    game.Run();
#if !DEBUG
        }
            catch (Exception e)
            {
                File.WriteAllText("error-" + DateTime.Now.Ticks + ".txt", e.ToString());
            }
#endif
        }
    }
}
