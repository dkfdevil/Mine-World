using Microsoft.Xna.Framework;
using MineWorld.GameStateManagers;

namespace MineWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MineWorldClient : Game
    {
        //Global variable definitions
        GameStateManager _gameStateManager;
        readonly GraphicsDeviceManager _graphics;

        public MineWorldClient()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _gameStateManager = new GameStateManager(_graphics, Content, this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _gameStateManager.LoadContent();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _gameStateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _gameStateManager.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
