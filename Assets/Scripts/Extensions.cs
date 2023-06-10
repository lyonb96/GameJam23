using UnityEngine;

public static class Extensions
{
    public static Vector2 Truncate(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 Expand(this Vector2 v, float z = 0.0F)
    {
        return new Vector3(v.x, v.y, z);
    }
}