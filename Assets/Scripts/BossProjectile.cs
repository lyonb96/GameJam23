using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float Direction;

    public float Speed = 5.0F;

    public float Lifetime = 3.0F;

    private bool HasDealtDamage;

    // Start is called before the first frame update
    void Start()
    {
        if (Direction < 0.0F)
        {
            transform.localScale *= -1.0F;
        }
        Destroy(gameObject, Lifetime);
    }

    private void Update()
    {
        transform.position += new Vector3(
            Direction * Speed * Time.deltaTime,
            0.0F,
            0.0F);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HasDealtDamage && collision.TryGetComponent<Player>(out var _))
        {
            HasDealtDamage = true;
            collision.GetComponent<Health>().Damage(1);
        }
    }
}
