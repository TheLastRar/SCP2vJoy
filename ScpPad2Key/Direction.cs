using System;

namespace ScpPad2vJoy
{
    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 1 << 1,   // 2
        Left = 1 << 2,   // 4
        Right = 1 << 3,   // 8
        UpLeft = Up | Left,
        DownLeft = Down | Left,
        UpRight = Up | Right,
        DownRight = Down | Right
    }
}
