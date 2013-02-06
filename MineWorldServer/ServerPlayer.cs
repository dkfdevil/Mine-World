using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace MineWorldServer
{
    public class ServerPlayer
    {
        public int Id;
        public string Name;
        public string Ip = "";
        public NetConnection NetConn;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Heading = Vector3.Zero;

        public ServerPlayer(NetConnection netcon)
        {
            NetConn = netcon;
            Id = GetId();
            Ip = netcon.RemoteEndpoint.Address.ToString();
        }

        private static int _id;
        public static int GetId()
        {
            _id = _id + 1;
            return _id;
        }
    }
}
