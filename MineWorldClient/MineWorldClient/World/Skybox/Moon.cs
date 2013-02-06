using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MineWorld.World.Skybox
{
    public class Moon
    {
        Texture2D _moonTex;
        VertexPositionTexture[] _moonArray;

        Effect _effect;

        float _fTime;

        Vector3 _playerpos;

        public void Load(ContentManager conmanager)
        {
            _moonTex = conmanager.Load<Texture2D>("Textures/moon");
            _effect = conmanager.Load<Effect>("Effects/DefaultEffect");

            _moonArray = new VertexPositionTexture[6];
            _moonArray[0] = new VertexPositionTexture(new Vector3(-0.5f, 0, -0.5f), new Vector2(0, 0));
            _moonArray[1] = new VertexPositionTexture(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0));
            _moonArray[2] = new VertexPositionTexture(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1));
            _moonArray[3] = new VertexPositionTexture(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0));
            _moonArray[4] = new VertexPositionTexture(new Vector3(0.5f, 0, 0.5f), new Vector2(1, 1));
            _moonArray[5] = new VertexPositionTexture(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1));
        }

        public void Update(float time,Vector3 pos)
        {
            _fTime = time;
            _playerpos = pos;
        }

        public void Draw(GraphicsDevice gDevice)
        {
            //MOON
            //Set the texture, set the world matrix to be the same thing as the sun, but negated
            _effect.Parameters["myTexture"].SetValue(_moonTex);
            _effect.Parameters["World"].SetValue(Matrix.CreateScale(50) * Matrix.CreateFromYawPitchRoll(0, 0, (float)((_fTime + MathHelper.PiOver2) + Math.PI)) * Matrix.CreateTranslation(_playerpos - new Vector3((float)(Math.Cos(_fTime) * 192), (float)(Math.Sin(_fTime) * 192), 0)));
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _moonArray, 0, 2); //Draw it
            }
        }
    }
}
