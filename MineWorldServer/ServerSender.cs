using Lidgren.Network;
using Lidgren.Network.Xna;
using MineWorldData;
using System.Net;

namespace MineWorldServer
{
    public class ServerSender
    {
        readonly MineWorldServer _mineserver;
        readonly NetServer _netserver;
        NetOutgoingMessage _outmsg;

        public ServerSender(NetServer nets,MineWorldServer mines)
        {
            _mineserver = mines;
            _netserver = nets;
        }

        public void SendInitialUpdate(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerInitialUpdate);
            _outmsg.Write(player.Id);
            _outmsg.Write(player.Name);
            _outmsg.Write(player.Position);
            _netserver.SendMessage(_outmsg, player.NetConn, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCurrentWorld(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.WorldMapSize);
            _outmsg.Write(_mineserver.MapManager.Mapsize);
            _netserver.SendMessage(_outmsg, player.NetConn, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerJoined(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerJoined);
            _outmsg.Write(player.Id);
            _outmsg.Write(player.Name);
            _outmsg.Write(player.Position);
            _outmsg.Write(player.Heading);
            foreach (ServerPlayer dummy in _mineserver.PlayerManager.PlayerList.Values)
            {
                if (player.Id != dummy.Id)
                {
                    _netserver.SendMessage(_outmsg, dummy.NetConn, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void SendOtherPlayersInWorld(ServerPlayer player)
        {
            foreach (ServerPlayer dummy in _mineserver.PlayerManager.PlayerList.Values)
            {
                if (player.Id != dummy.Id)
                {
                    _outmsg = _netserver.CreateMessage();
                    _outmsg.Write((byte)PacketType.PlayerJoined);
                    _outmsg.Write(dummy.Id);
                    _outmsg.Write(dummy.Name);
                    _outmsg.Write(dummy.Position);
                    _outmsg.Write(dummy.Heading);
                    _netserver.SendMessage(_outmsg, player.NetConn, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void SendPlayerLeft(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerLeft);
            _outmsg.Write(player.Id);
            foreach (ServerPlayer dummy in _mineserver.PlayerManager.PlayerList.Values)
            {
                if (player.Id != dummy.Id)
                {
                    _netserver.SendMessage(_outmsg, dummy.NetConn, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void SendMovementUpdate(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerMovementUpdate);
            _outmsg.Write(player.Id);
            _outmsg.Write(player.Position);
            foreach (ServerPlayer dummy in _mineserver.PlayerManager.PlayerList.Values)
            {
                if (player.Id != dummy.Id)
                {
                    _netserver.SendMessage(_outmsg, dummy.NetConn, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void SendNameSet(ServerPlayer player)
        {
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerMovementUpdate);
            _outmsg.Write(player.Id);
            _outmsg.Write(player.Name);
            foreach (ServerPlayer dummy in _mineserver.PlayerManager.PlayerList.Values)
            {
                _netserver.SendMessage(_outmsg, dummy.NetConn, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendDiscoverResponse(IPEndPoint endpoint)
        {
            // Create a response and write some example data to it
            _outmsg = _netserver.CreateMessage();
            _outmsg.Write(_mineserver.ServerName);
            _outmsg.Write(_mineserver.PlayerManager.PlayerList.Count);
            _outmsg.Write(_mineserver.Server.Configuration.MaximumConnections);
            _outmsg.Write(false);

            // Send the response to the sender of the request
            _netserver.SendDiscoveryResponse(_outmsg, endpoint);
        }
    }
}
