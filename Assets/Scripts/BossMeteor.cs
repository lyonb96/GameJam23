using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeteor : MonoBehaviour
{
    public Vector2 Direction;

    public float Speed = 5.0F;

    private bool HasDealtDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Speed * Time.deltaTime * Direction).Expand(0.0F);
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
