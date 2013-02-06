using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MineWorldServer
{
    class Program
    {
        private static void RunServer()
        {
            MineWorldServer mineWorldServer = new MineWorldServer();
            mineWorldServer.Start();
        }

        static void Main()
        {
            if (Debugger.IsAttached)
            {
                RunServer();
            }
            else
            {
                try
                {
                    RunServer();
                }
                catch (Exception e)
                {
                    if (!Directory.Exists("Crashlogs"))
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
