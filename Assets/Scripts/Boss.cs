using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Boss : MonoBehaviour
{
    public float MoveSpeed = 7.5F;

    public bool Pause;

    public float MinX = -1000.0F;

    public float MaxX = 1000.0F;

    public GameObject BossWavePrefab;

    private Health Health;

    private Player Player;

    private Animator Animator;

    private SpriteRenderer Sprite;

    private int Phase;

    private bool Attacking;

    private float MoveDirection;

    private bool RunningAway;

    private bool BossIsLeft => Player.transform.position.x > transform.position.x;

    // Start is called before the first frame update
    void Start()
    {
        Health = GetComponent<Health>();
        Health.OnDamage.AddListener(OnDamage);
        Health.OnDeath.AddListener(OnDeath);
        Player = FindObjectOfType<Player>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Animator.SetBool("Running", MoveDirection != 0.0F);
        if (Pause)
        {
            return;
        }
        if (!Attacking)
        {
            // Pick an attack
            var attack = Random.Range(0, Phase);
            switch (attack)
            {
                case 0:
                    StartCoroutine(RunAttack1());
                    break;
                case 1:
                    StartCoroutine(RunAttack2());
                    break;
                case 2:
                    StartCoroutine(RunAttack3());
                    break;
            }
        }
        var playerX = Player.transform.position.x;
        Sprite.flipX = !BossIsLeft;
        if (RunningAway)
        {
            var minAttackRange = 4.0F;
            var maxAttackRange = 7.0F;
            var farLeft = BossIsLeft
                ? playerX - maxAttackRange
                : playerX + minAttackRange;
            var farRight = BossIsLeft
                ? playerX - minAttackRange
                : playerX + maxAttackRange;
            if (transform.position.x < farLeft)
            {
                MoveDirection = 1.0F;
            }
            else if (transform.position.x > farRight)
            {
                MoveDirection = -1.0F;
            }
            else
            {
                MoveDirection = 0.0F;
            }
            var newX = transform.position.x + MoveDirection * MoveSpeed * Time.deltaTime;
            transform.position = transform.position.With(x: newX);
        }
    }

    IEnumerator RunAttack1()
    {
        Attacking = true;
        // Step 1: Run away from the player
        RunningAway = true;
        yield return new WaitForSeconds(1.5F);
        RunningAway = false;
        // Step 2: Charge the melee
        Animator.SetBool("Charging", true);
        yield return new WaitForSeconds(0.5F);
        Animator.SetBool("Charging", false);
        // Step 3: Swing and spawn projectile (spawn handled in animation callback)
        Animator.SetTrigger("Swing");
        // Step 4: Idle so the player can attack
        yield return new WaitForSeconds(5.0F);
        Attacking = false;
    }

    IEnumerator RunAttack2()
    {
        Attacking = true;
        // Step 1: Run away from the player
        RunningAway = true;
        yield return new WaitForSeconds(1.5F);
        RunningAway = false;
        // Step 2: Raise the sword
        Health.SetImmune(true);
        Animator.SetBool("Raise", true);
        // Spawn projectiles here
        yield return new WaitForSeconds(2.0F);
        Animator.SetBool("Raise", false);
        Health.SetImmune(false);
        // Step 3: Idle for a bit
        yield return new WaitForSeconds(5.0F);
        Attacking = false;
    }

    IEnumerator RunAttack3()
    {
        Attacking = true;
        // Find a spot to teleport and do the thing
        var teleports = GameObject.FindGameObjectsWithTag("BossTeleport");
        var teleport = Random.Range(0, teleports.Length);
        yield return Extensions.Fade(0.5F, 1.0F, 0.0F, opacity => Sprite.color = Sprite.color.WithAlpha(opacity));
        Health.SetImmune(true);
        yield return new WaitForSeconds(1.0F);
        transform.position = teleports[teleport].transform.position;
        Health.SetImmune(false);
        yield return Extensions.Fade(0.5F, 0.0F, 1.0F, opacity => Sprite.color = Sprite.color.WithAlpha(opacity));
        Attacking = false;
    }

    void OnDamage()
    {
        Animator.SetTrigger("Damage");
        if (Phase == 0 && Health.CurrentHealthPercent <= 0.66F)
        {
            Debug.Log("Phase 2 begins");
            Phase++;
        }
        else if (Phase == 1 && Health.CurrentHealthPercent <= 0.33F)
        {
            Debug.Log("Phase 3 begins");
            Phase++;
        }
    }

    void OnDeath()
    {
        // Notify the level script
    }

    public void DoSwing()
    {
        var attackOnRight = BossIsLeft;
        var attackCenter = transform.position.Truncate() + new Vector2(attackOnRight ? 1 : -1, 0);
        var attackExtents = new Vector2(2, 5);
        var hits = Extensions.CheckForHits(
            attackCenter,
            attackExtents,
            "Enemy");
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Player>(out var _))
            {
                hit.GetComponent<Health>().Damage(1);
            }
        }
        // Spawn projectile
        var projectile = Instantiate(BossWavePrefab, attackCenter + new Vector2(0.0F, 0.45F), Quaternion.identity);
        projectile.GetComponent<BossProjectile>().Direction = attackOnRight ? 1.0F : -1.0F;
    }
}
