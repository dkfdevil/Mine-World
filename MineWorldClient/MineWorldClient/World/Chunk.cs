using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MineWorld.GameStateManagers;
using MineWorld.World.Block;
using MineWorldData;
using System.IO;

namespace MineWorld.World
{
    public class Chunk
    {
        //All of these should be public
        private readonly WorldManager _world;
        private readonly GameStateManager _gameman;
        public List<VertexFormat> LVertices;
        public VertexBuffer Buffer;
        public VertexDeclaration Verdec;
        public BaseBlock[, ,] BlockMap;
        readonly Effect _effect;
        bool _bEmpty;
        bool _bDraw;
        //Containing our blocks from terrain.png
        public Texture2D Terrain;
        float _fTime;

        //These nummers are lowered by 1 cause a array starts at zero
        public static int Size = 16;
        public static int Height = 128;

        //We dont need a posy cause our height will always be the total height
        public int PosX;
        public int PosZ;

        //Constructor
        public Chunk(int x,int z, WorldManager gamein,GameStateManager man)
        {
            //Needed cause of the buffer copy
            _gameman = man;
            _world = gamein;

            PosX = x;
            PosZ = z;

            BlockMap = new BaseBlock[Size,Height,Size];

            //Load our effect
            _effect = _gameman.Conmanager.Load<Effect>("Effects/DefaultEffect");

            //Load our terrain
            //Check if the user want to use custom textures
            if (_world.Customtexturepath != "")
            {
                Terrain = TextureFromFile(_world.Customtexturepath);
            }
            else
            {
                Terrain = _gameman.Conmanager.Load<Texture2D>("Textures/terrain");
            }

            //Set unchanging effect parameters (Fog and a constant value used for lighting)
            _effect.Parameters["FogEnabled"].SetValue(true);
            _effect.Parameters["FogStart"].SetValue(256);
            _effect.Parameters["FogEnd"].SetValue(512);
            _effect.Parameters["FogColor"].SetValue(Color.SkyBlue.ToVector4());
            _effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
        }

        public void Update(Vector3 playerpos,float time)
        {
            _fTime = time;
            if (Vector3.Distance(new Vector3(PosX + Size, Height / 2, PosZ + Size), playerpos) < 256)//If the current chunk selected is within the distance
            {
                if (!_bDraw) //And if it isn't already loaded
                {
                    CreateVertices(); //Load it
                    _bDraw = true;
                }
            }
            else
            {
                if (_bDraw) //Otherwise if it is out of the distance and it IS loaded
                {
                    LVertices = null; //Unload it
                    Buffer = null;
                    _bDraw = false;
                }
            }
        }

        public void Draw(GraphicsDevice gDevice)
        {
            //CHUNKS
            //Select a rendering technique from the effect file
            _effect.CurrentTechnique = _effect.Techniques["Technique1"];
            //Set the view and projection matrices, as well as the texture
            _effect.Parameters["View"].SetValue(_world.Player.Cam.View);
            _effect.Parameters["Projection"].SetValue(_world.Player.Cam.Projection);
            _effect.Parameters["myTexture"].SetValue(Terrain);

            //Set lighting variables based off of fTime of day
            _effect.Parameters["DiffuseColor"].SetValue(new Vector4(0.5f, 0.5f, 0.5f, 1) + new Vector4(0.5f, 0.5f, 0.5f, 0) * MathHelper.Clamp((float)Math.Sin(_fTime) * 5, -1, 1));
            _effect.Parameters["Direction"].SetValue(new Vector3((float)(1 + ((_fTime + MathHelper.PiOver2) % MathHelper.TwoPi) * (-1 / Math.PI)), (float)-(Math.Abs(1 + ((_fTime + MathHelper.PiOver2) % MathHelper.TwoPi) * (-1 / Math.PI)) - 1) / 2, 0f));
            _effect.Parameters["AmbientColor"].SetValue(new Vector4(0.25f, 0.25f, 0.25f, 1) + new Vector4(0.2f, 0.2f, 0.2f, 0) * MathHelper.Clamp((float)Math.Sin(_fTime) * 5, -1, 1));

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes) //For each pass in the current technique
            {
                    if (_bDraw && !_bEmpty) //If the chunk is loaded and it isn't empty
                    {
                        _effect.Parameters["World"].SetValue(Matrix.CreateTranslation(PosX,0,PosZ)); //Transform it to a world position
                        //effect.Parameters["World"].SetValue(Matrix.Identity * Matrix.CreateTranslation(PosX, 0, PosZ));
                        pass.Apply();
                        gDevice.SetVertexBuffer(Buffer); //Load its data from its buffer
                        gDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Buffer.VertexCount / 3); //Draw it
                    }
            }
        }

        public void CreateVertices()
        {
            //Re-initialize the VertexFormat list
            LVertices = new List<VertexFormat>();

            for (int x = 0; x < Size; x++)
            {
                for (int z = 0; z < Size; z++)
                {
                    for (int y = 0; y < Height; y++) //For each block in the chunk
                    {
                        if (BlockMap[x, y, z].Transparent) //If it's transparent
                        {
                            if (
                                (
                                BlockMap[IntClamp(x + 1, 0, Size - 1), y, z].Transparent ||
                                BlockMap[IntClamp(x - 1, 0, Size - 1), y, z].Transparent ||
                                BlockMap[x, IntClamp(y + 1, 0, Height - 1), z].Transparent ||
                                BlockMap[x, IntClamp(y - 1, 0, Height - 1), z].Transparent ||
                                BlockMap[x, y, IntClamp(z + 1, 0, Size - 1)].Transparent ||
                                BlockMap[x, y, IntClamp(z - 1, 0, Size - 1)].Transparent //And if it's not completely surrounded
                                )
                                &&
                                BlockMap[x, y, z].Type != BlockTypes.Air //And if it's not air
                            )
                            {
                                CreateCubeVertices(x, y, z, BlockMap[x, y, z].Model); //Create vertices
                            }
                        }
                        else //If it's opaque
                        {
                            if (
                                BlockMap[IntClamp(x + 1, 0, Size - 1), y, z].Transparent ||
                                BlockMap[IntClamp(x - 1, 0, Size - 1), y, z].Transparent ||
                                BlockMap[x, IntClamp(y + 1, 0, Height - 1), z].Transparent ||
                                BlockMap[x, IntClamp(y - 1, 0, Height - 1), z].Transparent ||
                                BlockMap[x, y, IntClamp(z + 1, 0, Size - 1)].Transparent ||
                                BlockMap[x, y, IntClamp(z - 1, 0, Size - 1)].Transparent //And if it's not completely surrounded
                                )
                            {
                                CreateCubeVertices(x, y, z, BlockMap[x, y, z].Model); //Create vertices
                            }
                        }
                    }
                }
            }
            CopyToBuffers();
        }

        private void CreateCubeVertices(int x, int y, int z, BlockModel model)
        {
            bool left;
            bool right;
            bool up;
            bool down;
            bool forward;
            bool backward;
            BaseBlock currentblock = BlockMap[x, y, z]; //Get current block

            if (currentblock.Transparent) //Set boolean values if each face is visible or not - if it's transparent then it doesn't draw faces adjacent to blocks of the same type
            {
                up = (BlockMap[x, IntClamp(y + 1, 0, Height - 1), z] != currentblock && (y + 1) == IntClamp(y + 1, 0, Height - 1));
                forward = (BlockMap[x, y, IntClamp(z + 1, 0, Size - 1)] != currentblock && (z + 1) == IntClamp(z + 1, 0, Size - 1));
                left = (BlockMap[IntClamp(x - 1, 0, Size - 1), y, z] != currentblock && (x - 1) == IntClamp(x - 1, 0, Size - 1));
                right = (BlockMap[IntClamp(x + 1, 0, Size - 1), y, z] != currentblock && (x + 1) == IntClamp(x + 1, 0, Size - 1));
                backward = (BlockMap[x, y, IntClamp(z - 1, 0, Size - 1)] != currentblock && (z - 1) == IntClamp(z - 1, 0, Size - 1));
                down = (BlockMap[x, IntClamp(y - 1, 0, Height - 1), z] != currentblock && (y - 1) == IntClamp(y - 1, 0, Height - 1));
            }
            else //Set boolean values if each face is visible or not
            {
                up = (BlockMap[x, IntClamp(y + 1, 0, Height - 1), z].Transparent && (y + 1) == IntClamp(y + 1, 0, Height - 1));
                forward = (BlockMap[x, y, IntClamp(z + 1, 0, Size - 1)].Transparent && (z + 1) == IntClamp(z + 1, 0, Size - 1));
                left = (BlockMap[IntClamp(x - 1, 0, Size - 1), y, z].Transparent && (x - 1) == IntClamp(x - 1, 0, Size - 1));
                right = (BlockMap[IntClamp(x + 1, 0, Size - 1), y, z].Transparent && (x + 1) == IntClamp(x + 1, 0, Size - 1));
                backward = (BlockMap[x, y, IntClamp(z - 1, 0, Size - 1)].Transparent && (z - 1) == IntClamp(z - 1, 0, Size - 1));
                down = (BlockMap[x, IntClamp(y - 1, 0, Height - 1), z].Transparent && (y - 1) == IntClamp(y - 1, 0, Height - 1));
            }

            up = true;
            forward = true;
            left = true;
            right = true;
            backward = true;
            down = true;

            if (model == BlockModel.Cube)
            {
                if (up) //Fill the vertex list with VertexFormats, which take 3 floats as a position, a vector3 as a normal, and a vector2 as UV mapping for textures
                {
                    LVertices.Add(new VertexFormat(x, (y + 1), z, new Vector3(0, 1, 0), currentblock.UvMapTop));
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), z, new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 1), z, new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));
                }
                if (forward)
                {
                    LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward));
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(0f, 1f / 16f)));
                }
                if (left)
                {
                    LVertices.Add(new VertexFormat(x, (y + 1), z, new Vector3(-1, 0, 0), currentblock.UvMapLeft));
                    LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(0f, 1f / 16f)));
                }
                if (right)
                {
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight));
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 1), z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(0f, 1f / 16f)));
                }
                if (backward)
                {
                    LVertices.Add(new VertexFormat((x + 1), (y + 1), z, new Vector3(0, 0, -1), currentblock.UvMapBackward));
                    LVertices.Add(new VertexFormat(x, (y + 1), z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, (y + 1), z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(0f, 1f / 16f)));
                }
                if (down)
                {
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(0f, 1f / 16f)));
                }
            }
            else if (model == BlockModel.Cross)
            {
                LVertices.Add(new VertexFormat(x, (y + 1), z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop));
                LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat(x, y, z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat((x + 1), y, (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                LVertices.Add(new VertexFormat(x, y, z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat((x + 1), (y + 1), (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop));
                LVertices.Add(new VertexFormat(x, (y + 1), z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat((x + 1), y, (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat(x, (y + 1), z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat(x, y, z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                LVertices.Add(new VertexFormat((x + 1), y, (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));



                LVertices.Add(new VertexFormat((x + 1), (y + 1), z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop));
                LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat((x + 1), y, z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat(x, y, (z + 1), Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                LVertices.Add(new VertexFormat((x + 1), y, z, Vector3.Normalize(new Vector3(1, 0, 1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat(x, (y + 1), (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop));
                LVertices.Add(new VertexFormat((x + 1), (y + 1), z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat(x, y, (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                LVertices.Add(new VertexFormat((x + 1), (y + 1), z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                LVertices.Add(new VertexFormat((x + 1), y, z, Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                LVertices.Add(new VertexFormat(x, y, (z + 1), Vector3.Normalize(new Vector3(-1, 0, -1)), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));
            }
            if (model == BlockModel.Slab)
            {
                if (up) //Fill the vertex list with VertexFormats, which take 3 floats as a position, a vector3 as a normal, and a vector2 as UV mapping for textures
                {
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), z, new Vector3(0, 1, 0), currentblock.UvMapTop));
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), z, new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), z, new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), (z + 1), new Vector3(0, 1, 0), currentblock.UvMapTop + new Vector2(0f, 1f / 16f)));
                }
                if (forward)
                {
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward));
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, 0, 1), currentblock.UvMapForward + new Vector2(0f, 1f / 16f)));
                }
                if (left)
                {
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), z, new Vector3(-1, 0, 0), currentblock.UvMapLeft));
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, (y + 0.2f), (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(-1, 0, 0), currentblock.UvMapLeft + new Vector2(0f, 1f / 16f)));
                }
                if (right)
                {
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight));
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(1, 0, 0), currentblock.UvMapRight + new Vector2(0f, 1f / 16f)));
                }
                if (backward)
                {
                    LVertices.Add(new VertexFormat((x + 1), (y + 0.2f), z, new Vector3(0, 0, -1), currentblock.UvMapBackward));
                    LVertices.Add(new VertexFormat(x, (y + 0.2f), z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, (y + 0.2f), z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, z, new Vector3(0, 0, -1), currentblock.UvMapBackward + new Vector2(0f, 1f / 16f)));
                }
                if (down)
                {
                    LVertices.Add(new VertexFormat(x + 1, y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom));
                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x + 1, y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(0f, 1f / 16f)));

                    LVertices.Add(new VertexFormat(x, y, z, new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 0f)));
                    LVertices.Add(new VertexFormat(x, y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(1f / 16f, 1f / 16f)));
                    LVertices.Add(new VertexFormat((x + 1), y, (z + 1), new Vector3(0, -1, 0), currentblock.UvMapBottom + new Vector2(0f, 1f / 16f)));
                }
            }
        }

        private void CopyToBuffers()
        {
            _bEmpty = true; //Initialize bEmpty to true

            VertexFormat[] tempverticesarray; //Initialize a temporary array

            tempverticesarray = LVertices.ToArray(); //Turn the list into an array, fill the array with it

            if (tempverticesarray.Length > 0) //If the array isn't empty
            {
                Buffer = new VertexBuffer(_gameman.Device, VertexFormat.VertexDeclaration, tempverticesarray.Length, BufferUsage.WriteOnly); //Initialize the buffer
                Buffer.SetData(tempverticesarray); //Fill it
                _bEmpty = false; //Set bEmpty to false, because it's not empty
            }
        }

        private int IntClamp(int value, int min, int max)
        {
            //Same intclamp function, I just wanted it in this class too
            return (value < min ? min : (value > max ? max : value));
        }

        public void SetBlock(int x,int y,int z,BaseBlock block,bool updatechunk)
        {
            if (BlockMap[x,y,z] != block) //If the target block isn't already what you want it to be
            {
                BlockMap[x, y, z] = block;//Then set the block to be that type
                if (updatechunk)
                {
                    CreateVertices();
                }
            }
        }

        public BaseBlock GetBlockAtPoint(int x,int y,int z)
        {
            return BlockMap[x, y, z];
        }

        private Texture2D TextureFromFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            Texture2D t2D = Texture2D.FromStream(_gameman.Device, fs);
            fs.Close();
            return t2D;
        }
    }
}