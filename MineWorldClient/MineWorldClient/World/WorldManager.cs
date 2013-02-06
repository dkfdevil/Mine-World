using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MineWorld.Actor;
using MineWorld.GameStateManagers;
using MineWorld.GameStateManagers.Helpers;
using MineWorld.World.Block;
using MineWorld.World.Block.Special;
using MineWorld.World.Skybox;
using MineWorldData;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MineWorld.World
{
    public class WorldManager
    {
        //Our manager
        readonly GameStateManager _gamemanager;

        //Our fTime
        public float FTime = 0.201358f;

        //Our player for in the world
        public Player Player;

        //Our other players
        public Dictionary<int, ClientPlayer> Playerlist = new Dictionary<int, ClientPlayer>();

        //Our block world
        public int Mapsize;
        public Chunk[,] Chunks;
        public int ChunkX;
        public int ChunkZ;

        public BaseBlock[] Blocks;

        //Our sky
        readonly Sun _sun = new Sun();
        readonly Moon _moon = new Moon();

        //Our rasterstates
        RasterizerState _wired;
        RasterizerState _solid;

        //A bool to see if everything is loaded
        public bool Worldmaploaded;

        //Custom texture path
        public string Customtexturepath = "";

        //A bool for debug purpose
        public bool Debug;

        public WorldManager(GameStateManager manager,Player player)
        {
            _gamemanager = manager;
            Player = player;
        }

        public void Load(ContentManager conmanager)
        {
            //Load our blocks
            Blocks = new BaseBlock[(int)BlockTypes.Max];
            CreateBlockTypes();

            //Load our sun and moon
            _sun.Load(conmanager);
            _moon.Load(conmanager);

            //Setup our rasterstates
            _wired = new RasterizerState();
            _wired.CullMode = CullMode.CullCounterClockwiseFace;
            _wired.FillMode = FillMode.WireFrame;
            _solid = new RasterizerState();
            _solid.CullMode = CullMode.CullCounterClockwiseFace;
            _solid.FillMode = FillMode.Solid;
        }

        public void Start()
        {
            //This is called when we got all the info we need and then we construct a world on it
            //Initialize the chunk array
            ChunkX = Mapsize / Chunk.Size;
            ChunkZ = Mapsize / Chunk.Size;

            Chunks = new Chunk[ChunkX, ChunkZ];

            for (int x = 0; x < ChunkX; x++)
            {
                for (int z = 0; z < ChunkZ; z++)
                {
                    Chunks[x, z] = new Chunk(x * Chunk.Size, z * Chunk.Size, this, _gamemanager); //Create each chunk with its position and pass it the game object
                }
            }

            //Lets create our map
            CreateSimpleMap();

            Worldmaploaded = true;
        }

        public void Update(GameTime gamefTime,InputHelper input)
        {
            //fTime increases at rate of 1 ingame day/night cycle per 20 minutes (actual value is 0 at dawn, 0.5pi at noon, pi at dusk, 1.5pi at midnight, and 0 or 2pi at dawn again)
            FTime += (float)(Math.PI / 36000);
            FTime %= MathHelper.TwoPi;

            foreach (Chunk c in Chunks)
            {
                c.Update(Player.Position,FTime);
            }

            //Update our moon and sun for the correct offset
            _sun.Update(FTime, _gamemanager.Pbag.Player.Position);
            _moon.Update(FTime, _gamemanager.Pbag.Player.Position);

            if (_gamemanager.Game.IsActive)
            {
                if (input.IsNewPress((Keys)ClientKey.WireFrame))
                {
                    _gamemanager.Pbag.WireMode = !_gamemanager.Pbag.WireMode;
                }
            }
        }

        public void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            gDevice.Clear(Color.SkyBlue);

            //Set some draw things
            gDevice.DepthStencilState = DepthStencilState.None;
            gDevice.BlendState = BlendState.AlphaBlend;
            gDevice.SamplerStates[0] = SamplerState.PointWrap;
            gDevice.DepthStencilState = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.GreaterEqual,
                ReferenceStencil = 254,
                DepthBufferEnable = true
            };

            if (_gamemanager.Pbag.WireMode)
            {
                gDevice.RasterizerState = _wired;
            }
            else
            {
                gDevice.RasterizerState = _solid;
            }


            foreach (Chunk c in Chunks)
            {
                c.Draw(gDevice);
            }


            //After the blocks make sure fillmode = solid once again
            gDevice.RasterizerState = _solid;

            //Draw the other players
            foreach (ClientPlayer dummy in Playerlist.Values)
            {
                dummy.Draw(Player.Cam.View, Player.Cam.Projection);
            }

            //Draw our beautifull sky
            _sun.Draw(gDevice);
            _moon.Draw(gDevice);
        }

        public void CreateBlockTypes()
        {
            //0 - Air
            Blocks[(int)BlockTypes.Air] = new BaseBlock(new Vector2(),BlockModel.Cube,false,true,false,BlockTypes.Air);
            //1 - Stone
            Blocks[(int)BlockTypes.Stone] = new BaseBlock(new Vector2(1, 0), BlockModel.Cube, true, false, true, BlockTypes.Stone);
            //2 - Grass
            Blocks[(int)BlockTypes.Grass] = new BaseBlock(new Vector2(0, 0), BlockModel.Cube, true, false, true, BlockTypes.Grass);
            //3 - Dirt
            Blocks[(int)BlockTypes.Dirt] = new BaseBlock(new Vector2(2, 0), BlockModel.Cube, true, false, true, BlockTypes.Dirt);
            //8 - Water
            Blocks[(int)BlockTypes.Water] = new BaseBlock(new Vector2(13, 12), BlockModel.Cube, false, true, false, BlockTypes.Water);
            //20 - Glass
            Blocks[(int)BlockTypes.Glass] = new BaseBlock(new Vector2(1, 3), BlockModel.Cube, true, true, true, BlockTypes.Glass);
            //25 - Noteblock
            Blocks[(int)BlockTypes.Noteblock] = new MusicBlock(new Vector2(10, 4), new Vector2(11, 4), new Vector2(11, 4), new Vector2(11, 4), new Vector2(11, 4), new Vector2(11, 4), BlockModel.Cube, true, false, true, BlockTypes.Noteblock, _gamemanager.Conmanager);
            //78 - Snow
            Blocks[(int)BlockTypes.Snow] = new BaseBlock(new Vector2(2, 4), BlockModel.Slab, false, true, true, BlockTypes.Snow);
        }

        public void CreateSimpleMap()
        {
            //Empty the map first
            EmptyMap();
            //This code is for test purpose and will be removed
            for (int x = 0; x < Mapsize; x++)
            {
                for (int z = 0; z < Mapsize; z++)
                {
                    for (int y = 0; y < Chunk.Height; y++)
                    {
                        if (y < (Chunk.Height / 2))
                        {
                            SetMapBlock(x, y, z, BlockTypes.Dirt);
                        }
                        if (y == Chunk.Height / 2)
                        {
                            SetMapBlock(x, y, z, BlockTypes.Grass);
                        }
                        if (y == (Chunk.Height / 2) + 1)
                        {
                            SetMapBlock(x, y, z, BlockTypes.Snow);
                        }
                    }
                }
            }
        }

        public void EmptyMap()
        {
            //This code is for test purpose and will be removed
            for (int x = 0; x < Mapsize; x++)
            {
                for (int z = 0; z < Mapsize; z++)
                {
                    for (int y = 0; y < Chunk.Height; y++)
                    {
                        SetMapBlock(x, y, z, BlockTypes.Air);
                    }
                }
            }
        }

        public Chunk GetChunkAtPosition(int x, int z)
        {
            Chunk c = Chunks[x / Chunk.Size, z / Chunk.Size];

            return c;
        }

        public void SetBlock(int x,int y, int z, BlockTypes type)
        {
            ////If its outside the map then ignore it
            if (x < 0 || x >= Mapsize || y < 0 || y >= Chunk.Height || z < 0 || z >= Mapsize)
            {
                return;
            }

            Chunk c = GetChunkAtPosition(x, z);

            int xb,yb,zb;
            xb = (x % Chunk.Size);
            yb = y;
            zb = (z % Chunk.Size);
            c.SetBlock(xb, yb, zb, Blocks[(int)type],true);
        }

        public void SetMapBlock(int x, int y, int z, BlockTypes type)
        {
            if (PointWithinMap(new Vector3(x, y, z)))
            {
                Chunk c = GetChunkAtPosition(x, z);

                int xb, yb, zb;
                xb = (x % Chunk.Size);
                yb = y;
                zb = (z % Chunk.Size);
                c.SetBlock(xb, yb, zb, Blocks[(int)type], false);
            }
        }

        public BaseBlock BlockAtPoint(Vector3 pos)
        {
            if (PointWithinMap(pos))
            {
                Chunk c = GetChunkAtPosition((int)pos.X, (int)pos.Z);

                int xb, yb, zb;
                xb = ((int)pos.X % Chunk.Size);
                yb = (int)pos.Y;
                zb = ((int)pos.Z % Chunk.Size);
                return c.GetBlockAtPoint(xb, yb, zb);
            }
            return Blocks[(int)(BlockTypes.Air)];
        }

        public bool PointWithinMap(Vector3 pos)
        {
            ////If its outside the map then ignore it
            if (pos.X < 0 || pos.X >= Mapsize || pos.Y < 0 || pos.Y >= Chunk.Height || pos.Z < 0 || pos.Z >= Mapsize)
            {
                return false;
            }

            return true;
        }

        // Returns true if we are solid at this point.
        public bool SolidAtPoint(Vector3 point)
        {
            return BlockAtPoint(point).Type != BlockTypes.Air;
        }

        public bool SolidAtPointForPlayer(Vector3 point)
        {
            return !BlockPassibleForPlayer(BlockAtPoint(point));
        }

        private bool BlockPassibleForPlayer(BaseBlock block)
        {
            if (block.Type == BlockTypes.Air)
            {
                return true;
            }

            return false;
        }

        public bool Everythingloaded()
        {
            return Worldmaploaded;
        }
    }
}
