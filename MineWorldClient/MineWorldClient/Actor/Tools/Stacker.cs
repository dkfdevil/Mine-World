using MineWorld.World;
using Microsoft.Xna.Framework;

namespace MineWorld.Actor.Tools
{
    //This tool is just a test tool to show the possibilies
    public class Stacker : Tool
    {
        public WorldManager Worldmanager;
        public Player Player;

        public Stacker(Player player, WorldManager manager)
        {
            Worldmanager = manager;
            Player = player;
        }

        public override void Use()
        {
            if (Player.GotSelection())
            {
                Vector3 temppos = Player.GetFacingBlock();
                for (int height = (int)temppos.Y; height < Chunk.Height; height++)
                {
                    Vector3 pos = new Vector3(temppos.X,height,temppos.Z);
                    Worldmanager.SetBlock((int)pos.X,(int)pos.Y,(int)pos.Z, Player.Selectedblocktype);
                }
            }
        }
    }
}