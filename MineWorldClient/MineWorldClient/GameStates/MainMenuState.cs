using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace MineWorld.GameStates
{
    class MainMenuState : BaseState
    {
        readonly GameStateManager _gamemanager;
        Manager _guiman;
        Window _mainmenu;
        Button _play;
        Button _settings;
        Button _exit;

        public MainMenuState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _guiman = new Manager(_gamemanager.Game, _gamemanager.Graphics, "Default");
            _guiman.Initialize();

            _mainmenu = new Window(_guiman);
            _mainmenu.Init();
            _mainmenu.Resizable = false;
            _mainmenu.Movable = false;
            _mainmenu.CloseButtonVisible = false;
            _mainmenu.Text = "Main Menu";
            _mainmenu.Width = 300;
            _mainmenu.Height = 400;
            _mainmenu.Center();
            _mainmenu.Visible = true;
            _mainmenu.BorderVisible = true;
            //mainmenu.Cursor = guiman.Skin.Cursors["Default"].Resource;

            _play = new Button(_guiman);
            _play.Init();
            _play.Text = "Play";
            _play.Width = 200;
            _play.Height = 50;
            _play.Left = 50;
            _play.Top = 0;
            _play.Anchor = Anchors.Bottom;
            _play.Parent = _mainmenu;

            _settings = new Button(_guiman);
            _settings.Init();
            _settings.Text = "Settings";
            _settings.Width = 200;
            _settings.Height = 50;
            _settings.Left = 50;
            _settings.Top = 50;
            _settings.Anchor = Anchors.Bottom;
            _settings.Parent = _mainmenu;

            _exit = new Button(_guiman);
            _exit.Init();
            _exit.Text = "Exit";
            _exit.Width = 200;
            _exit.Height = 50;
            _exit.Left = 50;
            _exit.Top = 100;
            _exit.Anchor = Anchors.Bottom;
            _exit.Parent = _mainmenu;

            _guiman.Cursor = _guiman.Skin.Cursors["Default"].Resource;
            _guiman.Add(_mainmenu);

            _gamemanager.Game.IsMouseVisible = true;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime, InputHelper input)
        {
            _guiman.Update(gameTime);
            if (_gamemanager.Game.IsActive)
            {
                if (_play.Pushed)
                {
                    _play.Pushed = false;
                    //gamemanager.Pbag.ClientSender.SendJoinGame("127.0.0.1");
                    //gamemanager.SwitchState(GameStates.LoadingState);
                    _gamemanager.SwitchState(GameState.ServerBrowsingState);
                }
                if (_settings.Pushed)
                {
                    _settings.Pushed = false;
                    _gamemanager.SwitchState(GameState.SettingsState);
                }
                if (_exit.Pushed)
                {
                    _exit.Pushed = false;
                    _gamemanager.ExitGame();
                }
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            _guiman.BeginDraw(gameTime);
            _guiman.EndDraw();
        }
    }
}
