using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld.GameStates
{
    public abstract class BaseState
    {
        public GameStateManager Manager;
        public GameStateManagers.GameState AssociatedState;
        public bool Contentloaded;

        protected BaseState(GameStateManager manager, GameStateManagers.GameState associatedState)
        {
            Manager = manager;
            AssociatedState = associatedState;
        }

        public abstract void Unload();
        public abstract void LoadContent(ContentManager contentloader);
        public abstract void Update(GameTime gameTime, InputHelper input);
        public abstract void Draw(GameTime gameTime,GraphicsDevice gDevice,SpriteBatch sBtach);
    }
}
