using MineWorld.World;
using MineWorldData;
using Microsoft.Xna.Framework;

namespace MineWorld.Actor.Tools
{
    public class BlockRemover : Tool
    {
        public WorldManager Worldmanager;
        public Player Player;

        public BlockRemover(Player player, WorldManager manager)
        {
            Worldmanager = manager;
            Player = player;
        }

        public override void Use()
        {
            if (Player.GotSelection())
            {
                Vector3 block = Player.VAimBlock;
                Worldmanager.SetBlock((int)block.X,(int)block.Y,(int)block.Z, BlockTypes.Air);
            }
        }
    }
}
