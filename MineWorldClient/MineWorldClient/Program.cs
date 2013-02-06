using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MineWorld
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            using (MineWorldClient game = new MineWorldClient())
            {
                if (Debugger.IsAttached)
                {
                    game.Run();
                }
                else
                {
                    try
                    {
                        game.Run();
                    }
                    catch (Exception e)
                    {
                        if(!Directory.Exists("Crashlogs"))
                        {
                            Directory.CreateDirectory("Crashlogs");
                        }                   
                        File.WriteAllText("Crashlogs/" + DateTime.Now.ToString("hh-mm-ss-dd-mm-yyyy") + ".log", e.Message + "\r\n\r\n" + e.StackTrace);
                        MessageBox.Show("The game has crashed. The crash info has been written to the crashlog.",
                                        "Crash and Burn", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                        MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }
    }
}

