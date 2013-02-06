using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MineWorld.World.Skybox
{
    public class Sun
    {
        Texture2D _sunTex;
        VertexPositionTexture[] _sunArray;

        Effect _effect;

        float _fTime;
        Vector3 _playerpos;

        public void Load(ContentManager conmanager)
        {
            _sunTex = conmanager.Load<Texture2D>("Textures/sun");
            _effect = conmanager.Load<Effect>("Effects/DefaultEffect");

            _sunArray = new VertexPositionTexture[6];
            _sunArray[0] = new VertexPositionTexture(new Vector3(-0.5f, 0, -0.5f), new Vector2(0, 0));
            _sunArray[1] = new VertexPositionTexture(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0));
            _sunArray[2] = new VertexPositionTexture(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1));
            _sunArray[3] = new VertexPositionTexture(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0));
            _sunArray[4] = new VertexPositionTexture(new Vector3(0.5f, 0, 0.5f), new Vector2(1, 1));
            _sunArray[5] = new VertexPositionTexture(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1));
        }

        public void Update(float time,Vector3 pos)
        {
            _fTime = time;
            _playerpos = pos;
        }

        public void Draw(GraphicsDevice gDevice)
        {
            _effect.CurrentTechnique = _effect.Techniques["Technique2"]; //Switch to technique 2 (gui and skybox)

            //SUN
            //Set the sun texture and world matrix (which transforms its position and angle based off of the fTime of day
            _effect.Parameters["myTexture"].SetValue(_sunTex);
            //effect.Parameters["World"].SetValue(Matrix.CreateScale(50) * Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(180)) * Matrix.CreateTranslation(playerpos));
            _effect.Parameters["World"].SetValue(Matrix.CreateScale(50) * Matrix.CreateFromYawPitchRoll(0, 0, _fTime + MathHelper.PiOver2) * Matrix.CreateTranslation(_playerpos + new Vector3((float)(Math.Cos(_fTime) * 192), (float)(Math.Sin(_fTime) * 192), 0)));
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _sunArray, 0, 2); //Draw it
            }
        }
    }
}
