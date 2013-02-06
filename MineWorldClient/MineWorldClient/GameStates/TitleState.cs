using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld.GameStates
{
    class TitleState : BaseState
    {
        readonly GameStateManager _gamemanager;
        Song _introsong;
        bool _introstarted;
        Texture2D _background;
        Rectangle _size;

        public TitleState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _gamemanager.Game.IsMouseVisible = false;
            _introsong = contentloader.Load<Song>("Music/intro");
            _background = contentloader.Load<Texture2D>("Textures/States/titlestate");
            _size.Width = _gamemanager.Graphics.PreferredBackBufferWidth;
            _size.Height = _gamemanager.Graphics.PreferredBackBufferHeight;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime, InputHelper input)
        {
            if (!_introstarted)
            {
                _gamemanager.Audiomanager.PlaySong(_introsong, false);
                _introstarted = true;
            }
            if (_gamemanager.Game.IsActive)
            {
                if (input.AnyKeyPressed(true))
                {
                    _gamemanager.Audiomanager.StopPlaying();
                    _gamemanager.SwitchState(GameState.MainMenuState);
                }
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            gDevice.Clear(Color.Black);
            sBatch.Begin();
            sBatch.Draw(_background, _size, Color.White);
            sBatch.End();
        }
    }
}
