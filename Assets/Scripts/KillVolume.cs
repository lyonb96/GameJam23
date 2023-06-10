using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var health = collision.GetComponent<Health>();
        var player = collision.GetComponent<Player>();
        if (health != null && player != null)
        {
            health.Damage(100000);
        }
        if (player != null)
        {
            player.FellOutOfWorld();
        }
    }
}
