using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEnums
{
    public enum LevelType
    {
        SideScroll,
        TopDown
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Idle,
        Falling
    }
}
