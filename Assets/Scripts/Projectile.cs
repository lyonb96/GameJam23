using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 Direction;

    private float SpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        SpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnTime + 2.0F < Time.time)
        {
            Destroy(gameObject);
            return;
        }
        transform.position += (8.0F * Time.deltaTime * Direction).Expand(0.0F);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            return;
        }
        if (collision.TryGetComponent<Health>(out var health))
        {
            health.Damage(1);
        }
        Destroy(gameObject);
    }
}
