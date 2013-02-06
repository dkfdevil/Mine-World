using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//I included this because it contains the Byte4 type

namespace MineWorld.World
{
    public struct VertexFormat
    {
        //This is my own custom vertex format, which uses bytes for a position rather than vectors composed of floats
        private Vector4 _position;
        private Vector3 _normal;
        private Vector2 _texCoord;

        //The constructor takes a 3 byte position, a vector3 normal, and a vector2 UV map coordinate
        public VertexFormat(float x, float y, float z, Vector3 normal, Vector2 uv)
        {
            _position = new Vector4(x, y, z, 1);
            _normal = normal;
            _texCoord = uv;
        }


        //This helps pass info to the effect file
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 4 + sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
    }
}
