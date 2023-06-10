using System;
using System.Collections.Generic;
using System.Linq;
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

    public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }

    public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
    {
        return new Vector2(x ?? v.x, y ?? v.y);
    }

    public static IEnumerable<Collider2D> CheckForHits(
        Vector2 center,
        Vector2 extent,
        string ignoreTag = null,
        GameObject[] ignores = null)
    {
        ignores ??= Array.Empty<GameObject>();
        var hits = Physics2D.OverlapBoxAll(center, extent, 0.0F);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Health>() != null
                && !ignores.Contains(hit.gameObject)
                && (ignoreTag == null || !hit.CompareTag(ignoreTag)))
            {
                yield return hit;
            }
        }
    }
}

public class CooldownTimer
{
    public float Duration;

    private float LastUse;

    public void Use()
    {
        LastUse = Time.time;
    }

    public bool IsOnCooldown => LastUse + Duration > Time.time;
}