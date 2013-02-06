using MineWorld.World;
using Microsoft.Xna.Framework;

namespace MineWorld.Actor.Tools
{
    public class BlockAdder : Tool
    {
        public WorldManager Worldmanager;
        public Player Player;

        public BlockAdder(Player player,WorldManager manager)
        {
            Worldmanager = manager;
            Player = player;
        }

        public override void Use()
        {
            if (Player.GotSelection())
            {
                Vector3 block = Player.GetFacingBlock();
                Worldmanager.SetBlock((int)block.X,(int)block.Y,(int)block.Z, Player.Selectedblocktype);
            }
        }
    }
}
