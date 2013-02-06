using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;
using TomShane.Neoforce.Controls;

namespace MineWorld.GameStates
{
    public class SettingsState : BaseState
    {
        readonly GameStateManager _gamemanager;
        Manager _guiman;
        Window _settingsmenu;
        Button _back;
        TextBox _playername;
        ScrollBar _volume;

        public SettingsState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _guiman = new Manager(_gamemanager.Game, _gamemanager.Graphics, "Default");
            _guiman.Initialize();

            _settingsmenu = new Window(_guiman);
            _settingsmenu.Init();
            _settingsmenu.Resizable = false;
            _settingsmenu.Movable = false;
            _settingsmenu.CloseButtonVisible = false;
            _settingsmenu.Text = "Settings Menu";
            _settingsmenu.Width = 300;
            _settingsmenu.Height = 400;
            _settingsmenu.Center();
            _settingsmenu.Visible = true;
            _settingsmenu.BorderVisible = true;
            _settingsmenu.Cursor = _guiman.Skin.Cursors["Default"].Resource;

            _back = new Button(_guiman);
            _back.Init();
            _back.Text = "Go Back";
            _back.Width = 200;
            _back.Height = 50;
            _back.Left = 50;
            _back.Top = 300;
            _back.Anchor = Anchors.Bottom;
            _back.Parent = _settingsmenu;

            _playername = new TextBox(_guiman);
            _playername.Init();
            _playername.Text = _gamemanager.Pbag.Player.Name;
            _playername.Width = 200;
            _playername.Height = 50;
            _playername.Left = 50;
            _playername.Top = 0;
            _playername.Anchor = Anchors.Bottom;
            _playername.Parent = _settingsmenu;

            _volume = new ScrollBar(_guiman, Orientation.Horizontal);
            _volume.Init();
            //Todo check why volume.value is reseting it to 50 :S
            _volume.Value = _gamemanager.Audiomanager.GetVolume();
            _volume.Range = 100;
            _volume.PageSize = 10;
            _volume.StepSize = 1;
            _volume.Width = 200;
            _volume.Height = 50;
            _volume.Left = 50;
            _volume.Top = 50;
            _volume.Anchor = Anchors.Bottom;
            _volume.Parent = _settingsmenu;

            _guiman.Add(_settingsmenu);

            _gamemanager.Game.IsMouseVisible = true;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime, InputHelper input)
        {
            _guiman.Update(gameTime);
            if (_back.Pushed)
            {
                _back.Pushed = false;

                //Save all settings when back is pushed
                _gamemanager.Pbag.Player.Name = _playername.Text;
                _gamemanager.Audiomanager.SetVolume(_volume.Value);

                //Also save it to the file
                _gamemanager.SaveSettings();

                _gamemanager.SwitchState(GameState.MainMenuState);
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            _guiman.BeginDraw(gameTime);
            _guiman.EndDraw();
        }
    }
}