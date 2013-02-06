using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld.GameStates
{
    public enum ErrorMsg
    {
        Kicked,
        Banned,
        ServerFull,
        VersionMismatch,
        ServerShutdown,
        ServerRestart,
        Unkown,
    }

    public class ErrorState : BaseState
    {
        readonly GameStateManager _gamemanager;
        SpriteFont _myFont;
        public string Error;
        Vector2 _errorlocation;

        public ErrorState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _myFont = contentloader.Load<SpriteFont>("Fonts/DefaultFont");
            _gamemanager.Game.IsMouseVisible = false;
        }

        public override void Unload()
        {
        }

        public void SetError(ErrorMsg msg)
        {
            switch (msg)
            {
                case ErrorMsg.Banned:
                    {
                        Error = "You have been banned from the server";
                        break;
                    }
                case ErrorMsg.Kicked:
                    {
                        Error = "You have been kicked from the server";
                        break;
                    }
                case ErrorMsg.ServerFull:
                    {
                        Error = "The server is full";
                        break;
                    }
                case ErrorMsg.VersionMismatch:
                    {
                        Error = "Client/Server version mismatch";
                        break;
                    }
                case ErrorMsg.ServerShutdown:
                    {
                        Error = "Server has shutdown";
                        break;
                    }
                case ErrorMsg.ServerRestart:
                    {
                        Error = "Server is restarting";
                        break;
                    }
                case ErrorMsg.Unkown:
                    {
                        Error = "A unknown error has occured";
                        break;
                    }
            }
        }

        public override void Update(GameTime gameTime, InputHelper input)
        {
            if (_gamemanager.Game.IsActive)
            {
                if (input.AnyKeyPressed(true))
                {
                    _gamemanager.SwitchState(GameState.MainMenuState);
                }
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            _errorlocation = new Vector2(_gamemanager.Graphics.PreferredBackBufferWidth / 2, _gamemanager.Graphics.PreferredBackBufferHeight / 2);
            gDevice.Clear(Color.Black);
            sBatch.Begin();
            sBatch.DrawString(_myFont,Error,_errorlocation,Color.White);
            sBatch.End();
        }
    }
}