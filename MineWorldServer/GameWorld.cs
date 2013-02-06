namespace MineWorldServer
{
    public class GameWorld
    {
        readonly MineWorldServer _mineserver;

        public GameWorld(MineWorldServer mines)
        {
            _mineserver = mines;
        }

        public void GenerateSpawnPosition(ServerPlayer player)
        {
            player.Position = _mineserver.MapManager.GenerateSpawnPosition();
        }
    }
}
