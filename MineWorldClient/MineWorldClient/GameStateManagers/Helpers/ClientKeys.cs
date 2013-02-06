using Microsoft.Xna.Framework.Input;

namespace MineWorld.GameStateManagers.Helpers
{
    public enum ClientKey
    {
        MoveForward = Keys.W,
        MoveLeft = Keys.A,
        MoveRight = Keys.D,
        MoveBack = Keys.S,
        MoveUp = Keys.Space,
        MoveDown = Keys.LeftControl,
        Run = Keys.LeftShift,

        //Ugly need to find a better solution
        ActionOne = MouseButtons.LeftButton,
        ActionTwo = MouseButtons.RightButton,

        Debug = Keys.F1,
        WireFrame = Keys.F2,
        FullScreen = Keys.F3,
        Exit = Keys.Escape,
    }
}
