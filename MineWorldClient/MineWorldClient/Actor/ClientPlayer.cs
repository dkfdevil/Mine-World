using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MineWorld.Actor
{
    public class ClientPlayer
    {
        readonly Model _playermodel;

        public int Id;
        public string Name;
        public Vector3 Position;
        public Vector3 Heading;
        public float Temprot;
        public float Scale = 0.1f;

        public ClientPlayer(ContentManager conmanager)
        {
            //Model
            _playermodel = conmanager.Load<Model>("Models/player");
        }

        public void Draw(Matrix view, Matrix projection)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[_playermodel.Bones.Count];
            _playermodel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _playermodel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effectmodel in mesh.Effects)
                {
                    effectmodel.EnableDefaultLighting();
                    effectmodel.World = transforms[mesh.ParentBone.Index] * 
                        Matrix.CreateScale(Scale) *
                        Matrix.CreateRotationY(Temprot)
                        * Matrix.CreateTranslation(Position);
                    effectmodel.View = view;
                    effectmodel.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
