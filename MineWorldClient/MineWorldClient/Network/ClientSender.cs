using Lidgren.Network;
using Lidgren.Network.Xna;
using MineWorld.Actor;
using MineWorldData;
using Microsoft.Xna.Framework;

namespace MineWorld.Network
{
    public class ClientSender
    {
        readonly PropertyBag _pbag;
        readonly NetClient _client;
        NetOutgoingMessage _outmsg;

        public ClientSender(NetClient netc, PropertyBag pb)
        {
            _client = netc;
            _pbag = pb;
        }

        public void SendJoinGame(string ip)
        {
            //IPEndPoint serverEndPoint
            // Create our connect message.
            _outmsg = _client.CreateMessage();
            _outmsg.Write(Constants.MineworldclientVersion);
            _outmsg.Write(_pbag.Player.Name);
            _client.Connect(ip, Constants.MineworldPort, _outmsg);
        }

        public void SendPlayerInWorld()
        {
            _outmsg = _client.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerInWorld);
            _outmsg.Write(true);
            _client.SendMessage(_outmsg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMovementUpdate()
        {
            _outmsg = _client.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerMovementUpdate);
            _outmsg.Write(_pbag.Player.Position);
            _client.SendMessage(_outmsg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendBlockSet(BlockTypes type,Vector3 pos)
        {
            _outmsg = _client.CreateMessage();
            _outmsg.Write((byte)PacketType.PlayerBlockSet);
            _outmsg.Write(pos);
            _outmsg.Write((byte)type);
        }

        public void DiscoverLocalServers()
        {
            _client.DiscoverLocalPeers(Constants.MineworldPort);
        }
    }
}
