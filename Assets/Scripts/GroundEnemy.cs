using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    private Rigidbody2D RigidBody;
    private Health Health;
    private Animator Animator;
    private SpriteRenderer Sprite;

    private Player Player;

    public float AggroRange = 12.0F;

    public float AttackRange = 4.0F;

    public float BaseMoveSpeed = 100.0F;

    public float AttackMoveSpeed = 400.0F;

    private float DesiredMove;

    private bool Attacking;

    private bool AttackLeft;

    private float AttackStartX;

    private float AttackStartTime;

    private float AttackEndX;

    private bool Dying;

    private CooldownTimer StunTimer = new()
    {
        Duration = 0.4F,
    };

    private CooldownTimer AttackTimer = new()
    {
        Duration = 6.0F,
    };

    private CooldownTimer DamageTimer = new()
    {
        Duration = 3.0F,
    };

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Health = GetComponent<Health>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        Player = FindObjectOfType<Player>();
        Health.OnDeath.AddListener(OnDeath);
    }

    void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Health = GetComponent<Health>();
        Player = FindObjectOfType<Player>();
        Sprite = GetComponent<SpriteRenderer>();
        Player = FindObjectOfType<Player>();
        Health.OnDeath.AddListener(OnDeath);
    }

    void FixedUpdate()
    {
        if (DesiredMove == 0.0F || Dying)
        {
            return;
        }
        var moveSpeed = Attacking ? AttackMoveSpeed : BaseMoveSpeed;
        RigidBody.velocity = new Vector2(DesiredMove * Time.fixedDeltaTime * moveSpeed, RigidBody.velocity.y);
        if (Attacking && !DamageTimer.IsOnCooldown)
        {
            // Check for hit
            var hitCenter = transform.position.Truncate()
                + new Vector2(AttackLeft ? -1.0F : 1.0F, 0.5F);
            var hitExtents = new Vector2(1, 1);
            var hits = Extensions.CheckForHits(
                hitCenter,
                hitExtents,
                "Enemy");
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    player.GetComponent<Health>().Damage(1);
                    DamageTimer.Use();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Dying)
        {
            return;
        }
        var myPos = transform.position;
        if (Attacking)
        {
            DesiredMove = AttackLeft
                ? -1.0F
                : 1.0F;
            if ((AttackLeft && myPos.x < AttackEndX)
                || (!AttackLeft && myPos.x > AttackEndX)
                || AttackStartTime + 2.0F < Time.time)
            {
                SetAttacking(false);
            }
        }
        else
        {
            var playerPos = Player.transform.position;
            var dist = Mathf.Abs(playerPos.x - myPos.x);
            // Figure out if player is left or right
            var isLeft = myPos.x > playerPos.x;
            if (dist < AggroRange && dist > AttackRange)
            {
                // Figure out if player is left or right
                if (!isLeft)
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
                Attack(isLeft);
            }
            Sprite.flipX = !isLeft;
        }
        Animator.SetBool("Moving", DesiredMove != 0.0F && !Attacking);
    }

    public void OnDamage()
    {
        StunTimer.Use();
        Animator.SetTrigger("Damage");
        SetAttacking(false);
    }

    public void OnDeath()
    {
        Animator.SetTrigger("Die");
        Dying = true;
        RigidBody.simulated = false;
    }

    public void DeathAnimationComplete()
    {
        Destroy(gameObject);
        // Spawn a particle effect, etc...
    }

    public void Attack(bool left)
    {
        AttackTimer.Use();
        AttackLeft = left;
        SetAttacking(true);
        AttackStartX = transform.position.x;
        AttackEndX = transform.position.x + (left ? -6.0F : 6.0F);
        AttackStartTime = Time.time;
    }

    public void SetAttacking(bool attacking)
    {
        Attacking = attacking;
        Animator.SetBool("Attacking", attacking);
    }

    //public void CheckForHit()
    //{
    //    var hits = Extensions.CheckForHits(
    //        transform.position.Truncate(),
    //        new Vector2(2, 2),
    //        ignoreTag: "Enemy");
    //    foreach (var hit in hits)
    //    {
    //        var health = hit.GetComponent<Health>();
    //        health.Damage(1);
    //    }
    //}
}
