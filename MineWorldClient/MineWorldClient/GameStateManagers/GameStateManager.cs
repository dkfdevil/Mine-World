using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EasyConfig;
using MineWorld.Actor;
using MineWorld.GameStateManagers.Helpers;
using MineWorld.GameStates;
using MineWorldData;

namespace MineWorld.GameStateManagers
{
    public enum GameState
    {
        TitleState,
        MainMenuState,
        LoadingState,
        MainGameState,
        ErrorState,
        SettingsState,
        ServerBrowsingState,
    }

    public class GameStateManager
    {
        public MineWorldClient Game;
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public GraphicsDevice Device;

        public AudioManager Audiomanager;
        private readonly InputHelper _inputhelper;
        public ConfigFile Config;
        public ContentManager Conmanager;
        public PropertyBag Pbag;

        private readonly ErrorState _errorstate;
        private readonly ServerBrowsingState _serverbrowsingstate;
        private readonly BaseState[] _screens;

        private BaseState _curScreen;

        public GameStateManager(GraphicsDeviceManager man,ContentManager cman,MineWorldClient gam)
        {
            Audiomanager = new AudioManager();
            Config = new ConfigFile("data/settings.ini");
            _inputhelper = new InputHelper();
            Game = gam;
            Conmanager = cman;
            Graphics = man;
            Device = Graphics.GraphicsDevice;
            SpriteBatch = new SpriteBatch(Device);
            _screens = new BaseState[]
                           {
                               new TitleState(this, GameState.TitleState),
                               new MainMenuState(this, GameState.MainMenuState),
                               new LoadingState(this, GameState.LoadingState),
                               new MainGameState(this, GameState.MainGameState),
                               new SettingsState(this, GameState.SettingsState),
                               _serverbrowsingstate = new ServerBrowsingState(this, GameState.ServerBrowsingState),
                               _errorstate = new ErrorState(this, GameState.ErrorState)
                           };
            //curScreen = titlestate;
            Pbag = new PropertyBag(gam,this);

            //Set initial state in the manager itself
            SwitchState(GameState.TitleState);
        }

        public void LoadContent()
        {
            LoadSettings();
            foreach (BaseState screen in _screens)
                screen.LoadContent(Conmanager);
        }

        public void Update(GameTime gameTime)
        {
            Pbag.ClientListener.Update();
            _inputhelper.Update();
            _curScreen.Update(gameTime,_inputhelper);
        }

        public void SwitchState(GameState newState)
        {
            foreach (BaseState screen in _screens)
            {
                if (screen.AssociatedState == newState)
                {
                    //This is true for the first time
                    if (_curScreen != null)
                    {
                        //Call unload for our currentscreen
                        _curScreen.Unload();
                        _curScreen.Contentloaded = false;
                    }

                    //Switch our currentscreen to our new screen
                    _curScreen = screen;

                    //If our new screen content isnt loaded yet call it
                    if (_curScreen.Contentloaded == false)
                    {
                        _curScreen.LoadContent(Conmanager);
                        _curScreen.Contentloaded = true;
                    }
                    break;
                }
            }
        }

        public void SetErrorState(ErrorMsg msg)
        {
            _errorstate.SetError(msg);
            SwitchState(GameState.ErrorState);
        }

        public void Temperrormsg(string text)
        {
            _errorstate.Error = text;
            SwitchState(GameState.ErrorState);
        }

        public void AddServer(ServerInformation server)
        {
            _serverbrowsingstate.AddServer(server);
        }

        public void RemoveServer(ServerInformation server)
        {
            _serverbrowsingstate.RemoveServer(server);
        }

        public void ExitGame()
        {
            Game.Exit();
        }

        public void Draw(GameTime gameTime)
        {
            _curScreen.Draw(gameTime,Device,SpriteBatch);
        }

        public void LoadSettings()
        {
            Game.Window.Title = "MineWorldClient v" + Constants.MineworldclientVersion.ToString(CultureInfo.InvariantCulture);
            Game.Window.AllowUserResizing = true;
            Game.Window.ClientSizeChanged += WindowClientSizeChanged;

            Graphics.PreferredBackBufferHeight = Config.SettingGroups["Video"].Settings["Height"].GetValueAsInt();
            Graphics.PreferredBackBufferWidth = Config.SettingGroups["Video"].Settings["Width"].GetValueAsInt();
            Graphics.IsFullScreen = Config.SettingGroups["Video"].Settings["Fullscreen"].GetValueAsBool();
            Graphics.SynchronizeWithVerticalRetrace = Config.SettingGroups["Video"].Settings["Vsync"].GetValueAsBool();
            Graphics.PreferMultiSampling = Config.SettingGroups["Video"].Settings["Multisampling"].GetValueAsBool();
            Graphics.ApplyChanges();

            Audiomanager.SetVolume(Config.SettingGroups["Sound"].Settings["Volume"].GetValueAsInt());

            Pbag.Player.Name = Config.SettingGroups["Player"].Settings["Name"].GetValueAsString();

            Pbag.WorldManager.Customtexturepath = Config.SettingGroups["Game"].Settings["Customtexturepath"].GetValueAsString();
        }

        public void SaveSettings()
        {
            Config.SettingGroups["Video"].Settings["Height"].SetValue(Graphics.PreferredBackBufferHeight);
            Config.SettingGroups["Video"].Settings["Width"].SetValue(Graphics.PreferredBackBufferWidth);
            Config.SettingGroups["Video"].Settings["Fullscreen"].SetValue(Graphics.IsFullScreen);
            Config.SettingGroups["Video"].Settings["Vsync"].SetValue(Graphics.SynchronizeWithVerticalRetrace);
            Config.SettingGroups["Video"].Settings["Multisampling"].SetValue(Graphics.PreferMultiSampling);

            Config.SettingGroups["Sound"].Settings["Volume"].SetValue(Audiomanager.GetVolume());

            Config.SettingGroups["Player"].Settings["Name"].SetValue(Pbag.Player.Name);

            Config.Save("data/settings.ini");
        }

        void WindowClientSizeChanged(object sender, EventArgs e)
        {
            Graphics.PreferredBackBufferWidth = Game.Window.ClientBounds.Width;
            Graphics.PreferredBackBufferHeight = Game.Window.ClientBounds.Height;
        }
    }
}
