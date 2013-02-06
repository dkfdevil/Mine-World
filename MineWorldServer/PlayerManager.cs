using System.Collections.Generic;
using Lidgren.Network;

namespace MineWorldServer
{
    public class PlayerManager
    {
        public Dictionary<NetConnection, ServerPlayer> PlayerList = new Dictionary<NetConnection, ServerPlayer>();

        public void AddPlayer(ServerPlayer play)
        {
            PlayerList.Add(play.NetConn, play);
        }

        public void RemovePlayer(ServerPlayer play)
        {
            if(PlayerList.ContainsValue(play))
            {
                PlayerList.Remove(play.NetConn);
            }
        }

        public ServerPlayer GetPlayerByConnection(NetConnection conn)
        {
            foreach (ServerPlayer dummy in PlayerList.Values)
            {
                if (dummy.NetConn == conn)
                {
                    return dummy;
                }
            }

            return null;
        }

        public NetConnection GetConnectionByPlayer(ServerPlayer play)
        {
            foreach (ServerPlayer dummy in PlayerList.Values)
            {
                if (play == dummy)
                {
                    return dummy.NetConn;
                }
            }

            return null;
        }

        public void KickPlayerByName(string name)
        {
            foreach (ServerPlayer player in PlayerList.Values)
            {
                if (player.Name.ToLower() == name.ToLower())
                {
                    KickPlayer(player);
                }
            }
        }

        public void KickPlayer(ServerPlayer player)
        {
            //We dont need to remove the player here cause its handled in the serverlistener on status changed
            player.NetConn.Disconnect("kicked");
        }

        public void KickAllPlayers()
        {
            foreach (ServerPlayer player in PlayerList.Values)
            {
                KickPlayer(player);
            }
        }

        public void BanPlayerByName(string name)
        {
            foreach (ServerPlayer player in PlayerList.Values)
            {
                if (player.Name.ToLower() == name.ToLower())
                {
                    BanPlayer(player);
                }
            }
        }

        public void BanPlayer(ServerPlayer player)
        {
            //We dont need to remove the player here cause its handled in the serverlistener on status changed
            player.NetConn.Disconnect("banned");
        }

        public bool NameExists(string name)
        {
            foreach (ServerPlayer player in PlayerList.Values)
            {
                if (player.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        public void SetPlayerName(ServerPlayer player, string name)
        {
            foreach (ServerPlayer dummy in PlayerList.Values)
            {
                if (dummy == player)
                {
                    dummy.Name = name;
                }
            }
        }
    }
}
