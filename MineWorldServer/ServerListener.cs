using Lidgren.Network;
using Lidgren.Network.Xna;
using MineWorldData;
using System.Threading;
using Microsoft.Xna.Framework;

namespace MineWorldServer
{
    public class ServerListener
    {
        readonly MineWorldServer _mineserver;
        readonly NetServer _netserver;
        NetConnection _msgSender;

        public ServerListener(NetServer nets,MineWorldServer mines)
        {
            _mineserver = mines;
            _netserver = nets;
        }

        public void Start()
        {
            NetIncomingMessage packetin;
            while (true)
            {
                while ((packetin = _netserver.ReadMessage()) != null)
                {
                    _msgSender = packetin.SenderConnection;
                    switch (packetin.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            {
                                NetConnectionStatus status = (NetConnectionStatus)packetin.ReadByte();
                                //Player doesnt want to play anymore
                                if (status == NetConnectionStatus.Disconnected || status == NetConnectionStatus.Disconnecting)
                                {
                                    _mineserver.Console.ConsoleWrite(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender).Name + " Disconnected");
                                    _mineserver.ServerSender.SendPlayerLeft(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                    _mineserver.PlayerManager.RemovePlayer(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                }
                                break;
                            }
                        case NetIncomingMessageType.DiscoveryRequest:
                            {
                                _mineserver.ServerSender.SendDiscoverResponse(packetin.SenderEndpoint);
                                break;
                            }
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                //If the server is full then deny
                                if (_netserver.ConnectionsCount == _netserver.Configuration.MaximumConnections)
                                {
                                    _msgSender.Deny("serverfull");
                                    break;
                                }

                                int version = packetin.ReadInt32();
                                string name = packetin.ReadString();

                                //If the version is any other then the server deny
                                if(_mineserver.VersionMatch(version) == false)
                                {
                                    _msgSender.Deny("versionmismatch");
                                    break;
                                }

                                ServerPlayer dummy = new ServerPlayer(_msgSender);
                                if (_mineserver.PlayerManager.NameExists(name))
                                {
                                    name += dummy.Id;
                                }

                                dummy.Name = name;
                                _mineserver.PlayerManager.AddPlayer(dummy);
                                _msgSender.Approve();
                                Thread.Sleep(4000);
                                _mineserver.GameWorld.GenerateSpawnPosition(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                _mineserver.ServerSender.SendCurrentWorld(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                _mineserver.ServerSender.SendInitialUpdate(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                _mineserver.ServerSender.SendPlayerJoined(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                _mineserver.ServerSender.SendOtherPlayersInWorld(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                _mineserver.Console.ConsoleWrite(name + " Connected");
                                break;
                            }
                        case NetIncomingMessageType.Data:
                            {
                                PacketType dataType = (PacketType)packetin.ReadByte();

                                switch (dataType)
                                {
                                    case PacketType.PlayerMovementUpdate:
                                        {
                                            _mineserver.PlayerManager.GetPlayerByConnection(_msgSender).Position = packetin.ReadVector3();
                                            _mineserver.ServerSender.SendMovementUpdate(_mineserver.PlayerManager.GetPlayerByConnection(_msgSender));
                                            break;
                                        }
                                    case PacketType.PlayerBlockSet:
                                        {
                                            Vector3 pos = packetin.ReadVector3();
                                            _mineserver.MapManager.WorldMap[(int)pos.X, (int)pos.Y, (int)pos.Z] = (BlockTypes)packetin.ReadByte();
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}
