using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MineWorld.Actor.Tools;
using MineWorld.GameStateManagers.Helpers;
using MineWorld.World.Block;
using MineWorldData;
using Microsoft.Xna.Framework.Content;

namespace MineWorld.Actor
{
    public class Player
    {
        //I actually did split up public and private, just to keep things simple outside of this class file
        private const float Movespeed = 3.5f;
        private const float Gravity = -16.0f;

        public int Myid;
        public string Name;
        public Camera Cam;
        public bool Mousehasfocus;
        public Vector3 Position;
        public Vector3 VTestVector;
        public float Speed = 0.5f;
        public float Sensitivity = 1;
        public Vector3 VAim;
        public Vector3 VAimBlock;
        public VertexPositionTexture[] RFaceMarker;
        public Vector3 VPlayerVel;
        public bool BUnderwater;
        public bool NoClip = true;
        public int Selectedblock;
        public BlockTypes Selectedblocktype = BlockTypes.Air;

        public Tool LeftHand;
        public Tool RightHand;

        private readonly PropertyBag _game;

        //Drawing related vars
        Effect _effect;
        Texture2D _tGui;
        Texture2D _tWaterOverlay;
        Texture2D _tVignette;

        //Constructor
        public Player(PropertyBag gameIn)
        {
            Cam = new Camera(gameIn, new Vector3(0, 64, 0), new Vector3(0, 0, 0));
            _game = gameIn;
        }

        public void Load(ContentManager conmanager)
        {
            _effect = conmanager.Load<Effect>("Effects/DefaultEffect");
            conmanager.Load<SpriteFont>("Fonts/DefaultFont");
            _tGui = conmanager.Load<Texture2D>("Textures/gui");
            _tWaterOverlay = conmanager.Load<Texture2D>("Textures/water");
            _tVignette = conmanager.Load<Texture2D>("Textures/vignette");

            //Lets give our player 2 tools 1 for each hand
            LeftHand = new BlockAdder(this, _game.WorldManager);
            RightHand = new BlockRemover(this, _game.WorldManager);
            //LeftHand = new Stacker(this, game.WorldManager);
        }

        public void Update(GameTime gtime, InputHelper input)
        {
            if (_game.GameManager.Game.IsActive)
            {
                if (NoClip) //If noclipped
                {
                    if (input.IsCurPress(Keys.LeftShift))
                    {
                        Speed = 1.0f;
                    }
                    else
                    {
                        Speed = 0.5f;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveForward))
                    {
                        Position -= Cam.Forward * Speed;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveLeft))
                    {
                        Position -= Cam.Right * Speed;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveBack))
                    {
                        Position += Cam.Forward * Speed;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveRight))
                    {
                        Position += Cam.Right * Speed;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveUp))
                    {
                        Position += Vector3.Up * Speed;
                    }
                    if (input.IsCurPress((Keys)ClientKey.MoveDown))
                    {
                        Position += Vector3.Down * Speed;
                    }
                }
                else
                {
                    //if (input.IsCurPress(Keys.W))
                    //{
                    //    Position -= Cam.Forward * Speed;
                    //}
                    //if (input.IsCurPress(Keys.A))
                    //{
                    //    Position -= Cam.Right * Speed;
                    //}
                    //if (input.IsCurPress(Keys.S))
                    //{
                    //    Position += Cam.Forward * Speed;
                    //}
                    //if (input.IsCurPress(Keys.D))
                    //{
                    //    Position += Cam.Right * Speed;
                    //}

                    ////Execute standard movement
                    //Vector3 footPosition = Position + new Vector3(0f, -1.5f, 0f);
                    //Vector3 headPosition = Position + new Vector3(0f, 0.1f, 0f);
                    //Vector3 midPosition = Position + new Vector3(0f, -0.7f, 0f);

                    //if (game.WorldManager.BlockAtPoint(headPosition) == BlockTypes.Water)
                    //{
                    //    bUnderwater = true;
                    //}
                    //else
                    //{
                    //    bUnderwater = false;
                    //}

                    //vPlayerVel.Y += Gravity * (float)gtime.ElapsedGameTime.TotalSeconds;

                    //if (game.WorldManager.SolidAtPointForPlayer(footPosition) || game.WorldManager.SolidAtPointForPlayer(headPosition))
                    //{
                    //    BlockTypes standingOnBlock = game.WorldManager.BlockAtPoint(footPosition);
                    //    BlockTypes hittingHeadOnBlock = game.WorldManager.BlockAtPoint(headPosition);

                    //    // If the player has their head stuck in a block, push them down.
                    //    if (game.WorldManager.SolidAtPointForPlayer(headPosition))
                    //    {
                    //        int blockIn = (int)(headPosition.Y);
                    //        Position.Y = (blockIn - 0.15f);
                    //    }

                    //    // If the player is stuck in the ground, bring them out.
                    //    // This happens because we're standing on a block at -1.5, but stuck in it at -1.4, so -1.45 is the sweet spot.
                    //    if (game.WorldManager.SolidAtPointForPlayer(footPosition))
                    //    {
                    //        int blockOn = (int)(footPosition.Y);
                    //        Position.Y = (float)(blockOn + 1 + 1.45);
                    //    }

                    //    vPlayerVel.Y = 0;
                    //}
                }
            }

            //Re-initialize aim and aimblock vectors
            VAim = new Vector3();
            VAimBlock = new Vector3();

            //Check along a path stemming from the camera's forward if there is a collision
            for (float i = 0; i <= 6; i += 0.01f)
            {
                if (i < 5.5f)
                {
                    VAim = Cam.Position - Cam.Forward * i;
                    try
                    {
                        BaseBlock select = _game.WorldManager.BlockAtPoint(VAim);
                        if (select.AimSolid)
                        {
                            break; //If there is, break the loop with the current aim vector
                        }
                    }
                    catch
                    {
                        VAim = new Vector3(-10, -10, -10);
                        break;
                    }
                }
                else
                {
                    VAim = new Vector3(-10, -10, -10);
                }
            } //Otherwise set it to be an empty vector

            if (VAim != new Vector3(-10, -10, -10))
            {
                VAimBlock = new Vector3((int)Math.Floor(VAim.X), (int)Math.Floor(VAim.Y), (int)Math.Floor(VAim.Z)); //Get the aim block based off of that aim vector
            }

            Cam.Position = Position;

            if (_game.GameManager.Game.IsActive)
            {
                if (Mousehasfocus)
                {
                    if (!input.IsCurPress(Keys.LeftAlt))
                    {
                        Cam.Rotate( //Rotate the camera based off of mouse position, set mouse position to be screen center
                            MathHelper.ToRadians((input.MousePosition.Y - _game.GameManager.Device.DisplayMode.Height / 2) * Sensitivity * 0.1f),
                            MathHelper.ToRadians((input.MousePosition.X - _game.GameManager.Device.DisplayMode.Width / 2) * Sensitivity * 0.1f),
                            0.0f
                            );
                        Mouse.SetPosition(_game.GameManager.Device.DisplayMode.Width / 2, _game.GameManager.Device.DisplayMode.Height / 2);
                    }
                    else
                    {
                        Mousehasfocus = false;
                    }
                }
                else
                {
                    Mousehasfocus = true;
                }
            }
            else
            {
                Mousehasfocus = false;
            }

            CreateFaceMarker(); //Create the face marker's vertices - I need to redo this method

            if (input.IsNewPress((MouseButtons)ClientKey.ActionOne))
            {
                //We cant do a thing with empty hand
                if (LeftHand != null)
                {
                    LeftHand.Use();
                }
            }

            if (input.IsNewPress((MouseButtons)ClientKey.ActionTwo))
            {
                //We cant do a thing with empty hand
                if (RightHand != null)
                {
                    RightHand.Use();
                }
            }

            if (input.MouseScrollWheelVelocity < 0f)
            {
                Selectedblock -= 1;
                if (Selectedblock < 0)
                {
                    Selectedblock = 0;
                }
                Selectedblocktype = (BlockTypes)Selectedblock;
            }

            if (input.MouseScrollWheelVelocity > 0f)
            {
                Selectedblock += 1;
                if (Selectedblock > 63)
                {
                    Selectedblock = 63;
                }
                Selectedblocktype = (BlockTypes)Selectedblock;
            }

            //Update our camera
            Cam.Update();
            //Send our position to the server
            _game.ClientSender.SendMovementUpdate();
        }


        public void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            if (RFaceMarker != null)
            {
                //FACE MARKER
                //Set world matrix to be default
                _effect.Parameters["World"].SetValue(Matrix.Identity);
                //Set the texture to be the gui texture
                _effect.Parameters["myTexture"].SetValue(_tGui);

                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gDevice.DrawUserPrimitives(PrimitiveType.TriangleList, RFaceMarker, 0, 2); //Draw the face marker
                }
            }

            //////////////////////
            //   START THE 2D   //
            //////////////////////

            sBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone); //Start 2d rendering
            if (BUnderwater) //If underwater, apply several overlays
            {
                sBatch.Draw(_tWaterOverlay, new Rectangle(0, 0, gDevice.DisplayMode.Width, gDevice.DisplayMode.Height), Color.DarkBlue);
                sBatch.Draw(_tWaterOverlay, new Rectangle(0, 0, gDevice.DisplayMode.Width, gDevice.DisplayMode.Height), Color.White);
                sBatch.Draw(_tVignette, new Rectangle(0, 0, gDevice.DisplayMode.Width, gDevice.DisplayMode.Height), Color.White);
            }

            //Cursor!
            sBatch.Draw(_tGui, new Rectangle(_game.GameManager.Graphics.PreferredBackBufferWidth / 2 - 16, _game.GameManager.Graphics.PreferredBackBufferHeight / 2 - 16, 32, 32), new Rectangle(0, 0, 32, 32), Color.White);

            sBatch.End();
        }

        public bool GotSelection()
        {
            if (VAim == new Vector3(-10, -10, -10))
            {
                return false;
            }

            return true;
        }

        //private bool TryToMoveTo(Vector3 moveVector)
        //{
        //    // Build a "test vector" that is a little longer than the move vector.
        //    float moveLength = moveVector.Length();
        //    Vector3 testVector = moveVector;
        //    testVector.Normalize();
        //    testVector = testVector * (moveLength); // + 0.1f);

        //    // Apply this test vector.
        //    Vector3 movePosition = Position + testVector;
        //    Vector3 midBodyPoint = movePosition + new Vector3(0, -0.7f, 0);
        //    Vector3 lowerBodyPoint = movePosition + new Vector3(0, -1.4f, 0);

        //    if (!game.WorldManager.SolidAtPointForPlayer(movePosition) &&
        //        !game.WorldManager.SolidAtPointForPlayer(lowerBodyPoint) &&
        //        !game.WorldManager.SolidAtPointForPlayer(midBodyPoint))
        //    {
        //        testVector = moveVector;
        //        testVector.Normalize();
        //        testVector = testVector * (moveLength + 0.11f);
        //        // Makes sure the camera doesnt move too close to the block ;)
        //        movePosition = Position + testVector;
        //        midBodyPoint = movePosition + new Vector3(0, -0.7f, 0);
        //        lowerBodyPoint = movePosition + new Vector3(0, -1.4f, 0);

        //        if (!game.WorldManager.SolidAtPointForPlayer(movePosition) &&
        //            !game.WorldManager.SolidAtPointForPlayer(lowerBodyPoint) &&
        //            !game.WorldManager.SolidAtPointForPlayer(midBodyPoint))
        //        {
        //            //vPosition = vPosition + moveVector;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public void CreateFaceMarker()
        {
            //Create the face marker's vertices - I need to redo this method

            Vector3 vDifference = GetFacingBlock() - VAimBlock; //Get a difference vector to see where the facing block is local to the aim block
            RFaceMarker = new VertexPositionTexture[6]; //Initialize the array of vectors - we only need 6 to make a square

            //Check the differences and draw the face marker accordingly
            if (vDifference.X == -1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y + 1, VAimBlock.Z), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y + 1, VAimBlock.Z + 1), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y, VAimBlock.Z), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y + 1, VAimBlock.Z + 1), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y, VAimBlock.Z + 1), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X - 0.01f, VAimBlock.Y, VAimBlock.Z), new Vector2(0.5f, 1));
            }
            else if (vDifference.X == 1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y + 1, VAimBlock.Z + 1), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y + 1, VAimBlock.Z), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y, VAimBlock.Z + 1), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y + 1, VAimBlock.Z), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y, VAimBlock.Z), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1.01f, VAimBlock.Y, VAimBlock.Z + 1), new Vector2(0.5f, 1));
            }
            else if (vDifference.Y == -1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y - 0.01f, VAimBlock.Z + 1), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y - 0.01f, VAimBlock.Z), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y - 0.01f, VAimBlock.Z + 1), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y - 0.01f, VAimBlock.Z), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y - 0.01f, VAimBlock.Z), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y - 0.01f, VAimBlock.Z + 1), new Vector2(0.5f, 1));
            }
            else if (vDifference.Y == 1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1.01f, VAimBlock.Z), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1.01f, VAimBlock.Z + 1), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1.01f, VAimBlock.Z), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1.01f, VAimBlock.Z + 1), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1.01f, VAimBlock.Z + 1), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1.01f, VAimBlock.Z), new Vector2(0.5f, 1));
            }
            else if (vDifference.Z == 1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1, VAimBlock.Z + 1.01f), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1, VAimBlock.Z + 1.01f), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y, VAimBlock.Z + 1.01f), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1, VAimBlock.Z + 1.01f), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y, VAimBlock.Z + 1.01f), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y, VAimBlock.Z + 1.01f), new Vector2(0.5f, 1));
            }
            else if (vDifference.Z == -1)
            {
                RFaceMarker[0] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y + 1, VAimBlock.Z - 0.01f), new Vector2(0.5f, 0));
                RFaceMarker[1] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1, VAimBlock.Z - 0.01f), new Vector2(1, 0));
                RFaceMarker[2] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y, VAimBlock.Z - 0.01f), new Vector2(0.5f, 1));
                RFaceMarker[3] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y + 1, VAimBlock.Z - 0.01f), new Vector2(1, 0));
                RFaceMarker[4] = new VertexPositionTexture(new Vector3(VAimBlock.X, VAimBlock.Y, VAimBlock.Z - 0.01f), new Vector2(1, 1));
                RFaceMarker[5] = new VertexPositionTexture(new Vector3(VAimBlock.X + 1, VAimBlock.Y, VAimBlock.Z - 0.01f), new Vector2(0.5f, 1));
            }
        }

        public Vector3 GetFacingBlock()
        {
            //Initialize vectors and a float which will be used to sort out which axis is most different
            Vector3 vDifference;
            Vector3 vFacingBlock = new Vector3();
            float tempcomp = 0;
            vDifference = VAim - VAimBlock - new Vector3(0.5f, 0.5f, 0.5f); //Get aim vec local to aim block

            //This method works by getting on which axis the local aim position is greatest - i.e. which face the cursor is on
            if (Math.Abs(vDifference.X) > Math.Abs(tempcomp))
            {
                tempcomp = vDifference.X;
                vFacingBlock = new Vector3(VAimBlock.X + Math.Sign(vDifference.X), VAimBlock.Y, VAimBlock.Z);
            }
            if (Math.Abs(vDifference.Y) > Math.Abs(tempcomp))
            {
                tempcomp = vDifference.Y;
                vFacingBlock = new Vector3(VAimBlock.X, VAimBlock.Y + Math.Sign(vDifference.Y), VAimBlock.Z);
            }
            if (Math.Abs(vDifference.Z) > Math.Abs(tempcomp))
            {
                vFacingBlock = new Vector3(VAimBlock.X, VAimBlock.Y, VAimBlock.Z + Math.Sign(vDifference.Z));
            }

            return vFacingBlock;
        }
    }
}
