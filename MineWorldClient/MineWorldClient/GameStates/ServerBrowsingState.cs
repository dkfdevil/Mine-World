using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;
using TomShane.Neoforce.Controls;

namespace MineWorld.GameStates
{
    public class ServerBrowsingState : BaseState
    {
        readonly GameStateManager _gamemanager;
        Manager _guiman;
        Window _serverbrowsingmenu;
        ListBox _serversbox;
        readonly Dictionary<string, ServerInformation> _servers = new Dictionary<string,ServerInformation>();
        Button _join;
        Button _refresh;
        Button _back;

        public ServerBrowsingState(GameStateManager manager, GameState associatedState)
            : base(manager, associatedState)
        {
            _gamemanager = manager;
        }

        public override void LoadContent(ContentManager contentloader)
        {
            _guiman = new Manager(_gamemanager.Game, _gamemanager.Graphics, "Default");
            _guiman.Initialize();

            _serverbrowsingmenu = new Window(_guiman);
            _serverbrowsingmenu.Init();
            _serverbrowsingmenu.Resizable = false;
            _serverbrowsingmenu.Movable = false;
            _serverbrowsingmenu.CloseButtonVisible = false;
            _serverbrowsingmenu.Text = "Server Browser";
            _serverbrowsingmenu.Width = 300;
            _serverbrowsingmenu.Height = 400;
            _serverbrowsingmenu.Center();
            _serverbrowsingmenu.Visible = true;
            _serverbrowsingmenu.BorderVisible = true;

            _serversbox = new ListBox(_guiman);
            _serversbox.Init();
            //servers.SetPosition(50, 50);
            _serversbox.Left = 50;
            _serversbox.Top = 150;
            _serversbox.Width = 200;
            _serversbox.Height = 200;
            _serversbox.Anchor = Anchors.Bottom;
            _serversbox.Parent = _serverbrowsingmenu;

            _join = new Button(_guiman);
            _join.Init();
            _join.Text = "Join";
            _join.Width = 200;
            _join.Height = 50;
            _join.Left = 50;
            _join.Top = 0;
            _join.Anchor = Anchors.Bottom;
            _join.Parent = _serverbrowsingmenu;

            _refresh = new Button(_guiman);
            _refresh.Init();
            _refresh.Text = "Refresh";
            _refresh.Width = 200;
            _refresh.Height = 50;
            _refresh.Left = 50;
            _refresh.Top = 50;
            _refresh.Anchor = Anchors.Bottom;
            _refresh.Parent = _serverbrowsingmenu;

            _back = new Button(_guiman);
            _back.Init();
            _back.Text = "Back";
            _back.Width = 200;
            _back.Height = 50;
            _back.Left = 50;
            _back.Top = 100;
            _back.Anchor = Anchors.Bottom;
            _back.Parent = _serverbrowsingmenu;

            _guiman.Cursor = _guiman.Skin.Cursors["Default"].Resource;
            _guiman.Add(_serverbrowsingmenu);

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
                if (_join.Pushed)
                {
                    _join.Pushed = false;
                    //Make sure we have a valid selection
                    if (_serversbox.ItemIndex != -1)
                    {
                        string selectedserver;
                        selectedserver = _serversbox.Items[_serversbox.ItemIndex].ToString();

                        foreach (ServerInformation server in _servers.Values)
                        {
                            if (selectedserver == server.GetTag())
                            {
                                _gamemanager.Pbag.ClientSender.SendJoinGame(server.Ipaddress);
                                _gamemanager.SwitchState(GameState.LoadingState);
                            }
                        }
                    }
                }
                if (_refresh.Pushed)
                {
                    _refresh.Pushed = false;
                    _serversbox.Items.Clear();
                    _servers.Clear();
                    _gamemanager.Pbag.ClientSender.DiscoverLocalServers();
                }
                if (_back.Pushed)
                {
                    _back.Pushed = false;
                    _gamemanager.SwitchState(GameState.MainMenuState);
                }
            }
        }

        public void AddServer(ServerInformation server)
        {
            _servers.Add(server.GetTag(), server);
            _serversbox.Items.Add(server.GetTag());
        }

        public void RemoveServer(ServerInformation server)
        {
            _servers.Remove(server.GetTag());
            _serversbox.Items.Remove(server.GetTag());
        }

        public override void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            _guiman.BeginDraw(gameTime);
            _guiman.EndDraw();
        }
    }
}