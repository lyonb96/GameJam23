using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    private Rigidbody2D RigidBody;
    private CircleCollider2D Collider;
    private Health Health;
    private Animator Animator;

    private Player Player;

    public float AggroRange = 10.0F;

    public float AttackRange = 1.6F;

    private float DesiredMove;

    private CooldownTimer StunTimer = new()
    {
        Duration = 0.4F,
    };

    private CooldownTimer AttackTimer = new()
    {
        Duration = 1.0F,
    };

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CircleCollider2D>();
        Health = GetComponent<Health>();
        Animator = GetComponent<Animator>();
        Player = FindObjectOfType<Player>();
    }

    void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CircleCollider2D>();
        Health = GetComponent<Health>();
        Player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        if (DesiredMove == 0.0F)
        {
            return;
        }
        RigidBody.velocity = new Vector2(DesiredMove * Time.fixedDeltaTime * 100.0F, RigidBody.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        var playerPos = Player.transform.position;
        var myPos = transform.position;
        var dist = Vector3.Distance(playerPos, myPos);
        if (dist < AggroRange && dist > AttackRange)
        {
            // Figure out if player is left or right
            if (myPos.x < playerPos.x)
            {
                DesiredMove = 1.0F;
            }
            else
            {
                DesiredMove = -1.0F;
            }
        }
        else
        {
            DesiredMove = 0.0F;
        }
        if (dist <= AttackRange * 1.15F
            && !StunTimer.IsOnCooldown
            && !AttackTimer.IsOnCooldown)
        {
            Attack();
        }
    }

    public void OnDamage()
    {
        StunTimer.Use();
    }

    public void OnDeath()
    {
        Destroy(gameObject);
        // Spawn a particle effect, etc...
    }

    public void Attack()
    {
        AttackTimer.Use();
        Animator?.SetTrigger("Attack");
    }

    public void CheckForHit()
    {
        var hits = Extensions.CheckForHits(
            transform.position.Truncate(),
            new Vector2(2, 2),
            ignoreTag: "Enemy");
        foreach (var hit in hits)
        {
            var health = hit.GetComponent<Health>();
            health.Damage(1);
        }
    }
}
