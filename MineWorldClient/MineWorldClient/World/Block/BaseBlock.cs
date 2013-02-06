using Microsoft.Xna.Framework;
using MineWorldData;
using Microsoft.Xna.Framework.Graphics;

namespace MineWorld.World.Block
{
    public enum BlockModel
    {
        Cube = 0,
        Cross = 1,
        Slab = 2,
    }

    //This is the baseblock other block are derived from this class
    public class BaseBlock
    {
        //This class only contains UV map coordinates, and solid/transparent bools
        //The only constructor available requires all fields

        public Vector2 UvMapTop;
        public Vector2 UvMapRight;
        public Vector2 UvMapLeft;
        public Vector2 UvMapForward;
        public Vector2 UvMapBackward;
        public Vector2 UvMapBottom;
        public bool Solid;
        public bool Transparent;
        public bool AimSolid;
        public BlockModel Model;
        public BlockTypes Type;
        public Texture2D Icon;

        public BaseBlock(Vector2 top, Vector2 forward, Vector2 backward, Vector2 right, Vector2 left, Vector2 bottom, BlockModel modelIn, bool solidIn, bool transparentIn, bool aimSolidIn, BlockTypes typeIn)
        {
            UvMapTop = top / 16f;
            UvMapForward = forward / 16f;
            UvMapBackward = backward / 16f;
            UvMapLeft = left / 16f;
            UvMapRight = right / 16f;
            UvMapBottom = bottom / 16f;
            Model = modelIn;
            Transparent = transparentIn;
            Solid = solidIn;
            AimSolid = aimSolidIn;
            Type = typeIn;
        }

        public BaseBlock(Vector2 uvMapIn, BlockModel modelIn, bool solidIn, bool transparentIn, bool aimSolidIn, BlockTypes typeIn)
        {
            UvMapTop = uvMapIn / 16f;
            UvMapForward = uvMapIn / 16f;
            UvMapBackward = uvMapIn / 16f;
            UvMapLeft = uvMapIn / 16f;
            UvMapRight = uvMapIn / 16f;
            UvMapBottom = uvMapIn / 16f;
            Model = modelIn;
            Transparent = transparentIn;
            Solid = solidIn;
            AimSolid = aimSolidIn;
            Type = typeIn;
        }

        public virtual void OnUse()
        {
        }
    }
}
