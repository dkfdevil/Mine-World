using MineWorldData;
using Microsoft.Xna.Framework;

namespace MineWorldServer
{
    public class MapManager
    {
        public BlockTypes[, ,] WorldMap;
        public int Mapsize;
        public int ChunkHeight;
        public int ChuckSize;
        public int ChunkX;
        public int ChunkZ;

        public MapManager()
        {
            ChunkHeight = 128;
            ChuckSize = 16;
        }

        public void SetMapSize(int size)
        {
            Mapsize = size;
            ChunkX = size / ChuckSize;
            ChunkZ = size / ChuckSize;
        }

        public void GenerateCubeMap(BlockTypes type)
        {
            WorldMap = new BlockTypes[Mapsize, ChunkHeight, Mapsize];
            EmptyMap();

            for (int xi = 0; xi < Mapsize; xi++)
            {
                for (int zi = 0; zi < Mapsize; zi++)
                {
                    for (int yi = 0; yi < (ChunkHeight / 2); yi++)
                    {
                        WorldMap[xi, yi, zi] = type;
                    }
                }
            }
        }

        public void EmptyMap()
        {
            for (int xi = 0; xi < Mapsize; xi++)
            {
                for (int zi = 0; zi < Mapsize; zi++)
                {
                    for (int yi = 0; yi < ChunkHeight; yi++)
                    {
                        WorldMap[xi, yi, zi] = BlockTypes.Air;
                    }
                }
            }
        }

        public Vector3 GenerateSpawnPosition()
        {
            Vector3 pos;
            pos.X = Utils.RandGen.Next(0, Mapsize);
            pos.Z = Utils.RandGen.Next(0, Mapsize);
            pos.Y = ChunkHeight + 1;

            return pos;
        }

        //public Chunk GetChunkAtPosition(int x, int z)
        //{
        //    Chunk c = Chunks[x / Chunk.Size, z / Chunk.Size];

        //    return c;
        //}

        //public void SetBlock(int x, int y, int z, BlockTypes type)
        //{
        //    ////If its outside the map then ignore it
        //    if (x < 0 || x >= Mapsize || y < 0 || y >= Chunk.Height || z < 0 || z >= Mapsize)
        //    {
        //        return;
        //    }

        //    Chunk c = GetChunkAtPosition(x, z);

        //    int xb, yb, zb;
        //    xb = (x % Chunk.Size);
        //    yb = y;
        //    zb = (z % Chunk.Size);
        //    c.SetBlock(xb, yb, zb, Blocks[(int)type], true);
        //}
    }
}
