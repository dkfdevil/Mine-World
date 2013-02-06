using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MineWorld.Actor;
using MineWorld.GameStateManagers.Helpers;

namespace MineWorld
{
    // I wrote myself a little debugger to find out some issues that iam having
    public class Debug
    {
        public bool Enabled;
        SpriteFont _myFont;
        private readonly PropertyBag _game;

        public Debug(PropertyBag gameIn)
        {
            _game = gameIn;
        }

        public void Load(ContentManager conmanager)
        {
            _myFont = conmanager.Load<SpriteFont>("Fonts/DefaultFont");
        }

        public void Update(GameTime gameTime,InputHelper input)
        {
            if (_game.Game.IsActive)
            {
                if (input.IsNewPress((Keys)ClientKey.Debug))
                {
                    Enabled = !Enabled;
                }
            }
        }

        public void Draw(GameTime gameTime, GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            if (Enabled)
            {
                sBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                sBatch.DrawString(_myFont, "Position:" + _game.Player.Position.ToString(), new Vector2(0, 0), Color.Black);
                sBatch.DrawString(_myFont, "Angle:" + _game.Player.Cam.Angle.ToString(), new Vector2(0, 15), Color.Black);
                sBatch.DrawString(_myFont, "Selectedblock:" + _game.Player.VAimBlock.ToString(), new Vector2(0, 30), Color.Black);
                sBatch.DrawString(_myFont, "Selectedblocktype:" + _game.Player.Selectedblocktype.ToString(), new Vector2(0, 45), Color.Black);
                sBatch.DrawString(_myFont, "Time:" + _game.WorldManager.FTime.ToString(CultureInfo.InvariantCulture), new Vector2(0, 60), Color.Black);
                sBatch.DrawString(_myFont, "Ping:" + _game.Client.ServerConnection.AverageRoundtripTime.ToString(CultureInfo.InvariantCulture), new Vector2(0, 75), Color.Black);
                sBatch.DrawString(_myFont, "Bytessend:" + _game.Client.ServerConnection.Statistics.SentBytes.ToString(CultureInfo.InvariantCulture), new Vector2(0, 90), Color.Black);
                sBatch.DrawString(_myFont, "Bytesreceived:" + _game.Client.ServerConnection.Statistics.ReceivedBytes.ToString(CultureInfo.InvariantCulture), new Vector2(0, 105), Color.Black);
                sBatch.End();
            }
        }
    }
}
