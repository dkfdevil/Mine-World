using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld.GameStates
{
    public class LoadingState : BaseState
    {
        readonly GameStateManager _gamemanager;
        Texture2D _loadingimg;
        float _loadingangle;
        Vector2 _loadingorgin;
        Vector2 _loadinglocation;
        Rectangle _loadingrectangle;


        public LoadingState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _loadingimg = contentloader.Load<Texture2D>("Textures/States/loading");
            _loadingorgin = new Vector2(_loadingimg.Width / 2, _loadingimg.Height / 2);
            _loadinglocation = new Vector2(_gamemanager.Graphics.PreferredBackBufferWidth / 2, _gamemanager.Graphics.PreferredBackBufferHeight / 2);
            _loadingrectangle = new Rectangle(0, 0, _loadingimg.Width, _loadingimg.Height);

            _gamemanager.Game.IsMouseVisible = false;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime, InputHelper input)
        {
            _loadingangle += 0.01f;
            if (input.IsNewPress((Keys)ClientKey.Exit))
            {
                _gamemanager.SwitchState(GameState.MainMenuState);
            }

            //If everything is loaded then lets play
            if (_gamemanager.Pbag.WorldManager.Everythingloaded())
            {
                _gamemanager.SwitchState(GameState.MainGameState);
                //gamemanager.Pbag.ClientSender.SendPlayerInWorld();
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            _loadinglocation = new Vector2(_gamemanager.Graphics.PreferredBackBufferWidth / 2, _gamemanager.Graphics.PreferredBackBufferHeight / 2);
            gDevice.Clear(Color.Black);
            sBatch.Begin();
            sBatch.Draw(_loadingimg, _loadinglocation, _loadingrectangle, Color.White, _loadingangle, _loadingorgin, 1.0f, SpriteEffects.None, 1);
            sBatch.End();
        }
    }
}