using Lidgren.Network;
using Lidgren.Network.Xna;
using MineWorld.Actor;
using MineWorld.GameStates;
using MineWorldData;

namespace MineWorld.Network
{
    public class ClientListener
    {
        readonly PropertyBag _pbag;
        readonly NetClient _client;
        NetIncomingMessage _msgBuffer;

        public ClientListener(NetClient netc, PropertyBag pb)
        {
            _client = netc;
            _pbag = pb;
        }

        public void Update()
        {
            // Recieve messages from the server.
            while ((_msgBuffer = _client.ReadMessage()) != null)
            {
                NetIncomingMessageType msgType = _msgBuffer.MessageType;

                switch (msgType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)_msgBuffer.ReadByte();

                            if (status == NetConnectionStatus.Disconnected)
                            {
                                string reason = _msgBuffer.ReadString();
                                switch (reason)
                                {
                                    case "kicked":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.Kicked);
                                            break;
                                        }
                                    case "banned":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.Banned);
                                            break;
                                        }
                                    case "serverfull":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.ServerFull);
                                            break;
                                        }
                                    case "versionmismatch":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.VersionMismatch);
                                            break;
                                        }
                                    case "shutdown":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.ServerShutdown);
                                            break;
                                        }
                                    case "restart":
                                        {
                                            _pbag.GameManager.SetErrorState(ErrorMsg.ServerRestart);
                                            break;
                                        }
                                    case "exit":
                                        {
                                            //Todo need to find more info about this case in the disconnect switch
                                            break;
                                        }
                                    default:
                                        {
                                            _pbag.GameManager.Temperrormsg(reason);
                                            //Pbag.GameManager.SetErrorState(ErrorMsg.Unkown);
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case NetIncomingMessageType.DiscoveryResponse:
                        {
                            string name = _msgBuffer.ReadString();
                            int playercount = _msgBuffer.ReadInt32();
                            int playermax = _msgBuffer.ReadInt32();
                            bool lan = _msgBuffer.ReadBoolean();
                            string ip = _msgBuffer.SenderEndpoint.Address.ToString();
                            ServerInformation newserver = new ServerInformation(name,ip,playercount,playermax,lan);
                            _pbag.GameManager.AddServer(newserver);
                            break;
                        }
                    case NetIncomingMessageType.ConnectionApproval:
                        {
                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            PacketType dataType = (PacketType)_msgBuffer.ReadByte();
                            switch (dataType)
                            {
                                case PacketType.WorldMapSize:
                                    {
                                        _pbag.WorldManager.Mapsize = _msgBuffer.ReadInt32();
                                        _pbag.WorldManager.Start();
                                        break;
                                    }
                                case PacketType.PlayerInitialUpdate:
                                    {
                                        _pbag.Player.Myid = _msgBuffer.ReadInt32();
                                        _pbag.Player.Name = _msgBuffer.ReadString();
                                        _pbag.Player.Position = _msgBuffer.ReadVector3();
                                        break;
                                    }
                                case PacketType.PlayerJoined:
                                    {
                                        ClientPlayer dummy = new ClientPlayer(_pbag.GameManager.Conmanager);
                                        dummy.Id = _msgBuffer.ReadInt32();
                                        dummy.Name = _msgBuffer.ReadString();
                                        dummy.Position = _msgBuffer.ReadVector3();
                                        dummy.Heading = _msgBuffer.ReadVector3();
                                        _pbag.WorldManager.Playerlist.Add(dummy.Id, dummy);
                                        break;
                                    }
                                case PacketType.PlayerLeft:
                                    {
                                        int id = _msgBuffer.ReadInt32();
                                        _pbag.WorldManager.Playerlist.Remove(id);
                                        break;
                                    }
                                case PacketType.PlayerMovementUpdate:
                                    {
                                        int id = _msgBuffer.ReadInt32();
                                        if (_pbag.WorldManager.Playerlist.ContainsKey(id))
                                        {
                                            _pbag.WorldManager.Playerlist[id].Position = _msgBuffer.ReadVector3();
                                        }
                                        break;
                                    }
                                case PacketType.PlayerNameSet:
                                    {
                                        int id = _msgBuffer.ReadInt32();
                                        //Lets see if its my id or someones else id
                                        if (_pbag.Player.Myid == id)
                                        {
                                            _pbag.Player.Name = _msgBuffer.ReadString();
                                        }
                                        else
                                        {
                                            //Then its someones else its id
                                            if (_pbag.WorldManager.Playerlist.ContainsKey(id))
                                            {
                                                _pbag.WorldManager.Playerlist[id].Position = _msgBuffer.ReadVector3();
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        break;
                }
            }
        }
    }
}
