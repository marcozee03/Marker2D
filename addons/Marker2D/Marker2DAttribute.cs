using System;
using Godot;
public class Marker2DAttribute : Attribute
{
    public static bool IsValidType(Type type)
    {
        if (type == typeof(Vector2))
        {
            return true;
        }
        else return false;
    }
}