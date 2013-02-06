using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld.GameStates
{
    public class MainGameState : BaseState
    {
        public GameStateManager Gamemanager;

        public MainGameState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            Gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            //Load our world
            Gamemanager.Pbag.WorldManager.Load(contentloader);
            //Load our Player
            Gamemanager.Pbag.Player.Load(contentloader);
            //Load our debugger
            Gamemanager.Pbag.Debugger.Load(contentloader);
        }

        public override void Unload()
        {
            Gamemanager.Pbag.WorldManager.Worldmaploaded = false;
        }

        public override void Update(GameTime gameTime,InputHelper input)
        {
            if (Gamemanager.Game.IsActive)
            {
                //Lets see if we need to end this game
                if (input.IsNewPress((Keys)ClientKey.Exit))
                {
                    Gamemanager.Pbag.Client.Disconnect("exit");
                    Gamemanager.SwitchState(GameState.MainMenuState);
                }

                if (input.IsNewPress((Keys)ClientKey.FullScreen))
                {
                    Gamemanager.Graphics.ToggleFullScreen();
                }
            }

            //Update chunks to load close ones, unload far ones
            Gamemanager.Pbag.WorldManager.Update(gameTime,input);
            Gamemanager.Pbag.Player.Update(gameTime, input);
            Gamemanager.Pbag.Debugger.Update(gameTime, input);
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            Gamemanager.Pbag.WorldManager.Draw(gameTime, gDevice, sBatch);
            Gamemanager.Pbag.Player.Draw(gameTime, gDevice, sBatch);
            Gamemanager.Pbag.Debugger.Draw(gameTime, gDevice, sBatch);
        }
    }
}