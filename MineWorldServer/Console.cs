using System;

namespace MineWorldServer
{
    public class MineWorldConsole
    {
        readonly MineWorldServer _mineserver;

        public MineWorldConsole(MineWorldServer mines)
        {
            _mineserver = mines;
            Console.TreatControlCAsInput = true;
        }

        public void ProcessInput(string input)
        {
            string[] inputsplitted = input.Split(new[] { ' ' });

            switch (inputsplitted[0].ToLower())
            {
                case "kick":
                    {
                        if (inputsplitted.Length == 2)
                        {
                            _mineserver.PlayerManager.KickPlayerByName(inputsplitted[1]);
                        }
                        else
                        {
                            ConsoleWrite("Specify player to be kicked");
                        }
                        break;
                    }
                case "exit":
                    {
                        if (inputsplitted.Length == 2)
                        {
                            _mineserver.ShutdownServer(int.Parse(inputsplitted[1]));
                        }
                        else
                        {
                            _mineserver.ShutdownServer(0);
                        }
                        break;
                    }
                case "restart":
                    {
                        if (inputsplitted.Length == 2)
                        {
                            _mineserver.RestartServer(int.Parse(inputsplitted[1]));
                        }
                        else
                        {
                            _mineserver.RestartServer(0);
                        }
                        break;
                    }
                default:
                    {
                        WriteError("Cant process input");
                        break;
                    }
            }
        }

        public void WriteError(string error)
        {
            ConsoleWrite(error, ConsoleColor.Red);
        }

        public void ConsoleWrite(string text)
        {
            Console.WriteLine(text);
        }

        public void ConsoleWrite(string text, ConsoleColor color)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public void SetTitle(string title)
        {
            Console.Title = title;
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public bool KeyAvailable()
        {
            return Console.KeyAvailable;
        }
    }
}
