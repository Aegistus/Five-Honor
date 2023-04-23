using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuardDirection
{
    None, Top, Left, Right
}

public static class GuardDirectionExt
{
    public static bool Counters(this GuardDirection direction, GuardDirection other)
    {
        if (direction == GuardDirection.Top && other == GuardDirection.Top)
        {
            return true;
        }
        if (direction == GuardDirection.Left && other == GuardDirection.Right)
        {
            return true;
        }
        if (direction == GuardDirection.Right && other == GuardDirection.Left)
        {
            return true;
        }
        return false;
    }
}
