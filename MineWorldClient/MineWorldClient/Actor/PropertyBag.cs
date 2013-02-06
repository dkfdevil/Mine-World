using Lidgren.Network;
using MineWorld.GameStateManagers;
using MineWorld.Network;
using MineWorld.World;
using MineWorldData;

namespace MineWorld.Actor
{
    public class PropertyBag
    {
        public MineWorldClient Game;
        public GameStateManager GameManager;
        public WorldManager WorldManager;
        public Player Player;
        public Debug Debugger;
        public NetClient Client;
        public ClientListener ClientListener;
        public ClientSender ClientSender;
        public NetIncomingMessage MsgBuffer;
        public BlockTypes[, ,] Tempblockmap;

        public bool WireMode;

        public PropertyBag(MineWorldClient gamein,GameStateManager gameManagerin)
        {
            Game = gamein;
            GameManager = gameManagerin;
            NetPeerConfiguration netconfig = new NetPeerConfiguration("MineWorld");
            netconfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            Client = new NetClient(netconfig);
            Client.Start();
            Player = new Player(this);
            WorldManager = new WorldManager(GameManager, Player);
            ClientListener = new ClientListener(Client, this);
            ClientSender = new ClientSender(Client, this);
            Debugger = new Debug(this);
        }
    }
}
