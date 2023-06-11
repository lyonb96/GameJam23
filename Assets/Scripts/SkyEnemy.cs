using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyEnemy : MonoBehaviour
{
    private Animator Animator;
    private Player Player;
    private Health Health;

    public float AggroRange = 12.0F;

    public float StrafeSpeed = 5.0F;

    public GameObject Projectile;

    public bool StayStill;

    private bool Aggro;

    private bool MovingLeft;

    public CooldownTimer AttackCooldown = new()
    {
        Duration = 6.0F,
    };

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        Health = GetComponent<Health>();
        Player = FindObjectOfType<Player>();
        Health.OnDamage.AddListener(OnDamage);
        Health.OnDeath.AddListener(OnDeath);
    }

    // Update is called once per frame
    void Update()
    {
        var distX = Mathf.Abs(Player.transform.position.x - transform.position.x);
        if (!Aggro)
        {
            if (distX < AggroRange)
            {
                // Aggro to the player and move around above them
                Aggro = true;
            }
        }
        else
        {
            if (!StayStill)
            {
                var minX = Player.transform.position.x - 5.0F;
                var maxX = Player.transform.position.x + 5.0F;
                if (MovingLeft)
                {
                    transform.position = transform.position.With(
                        x: transform.position.x - StrafeSpeed * Time.deltaTime);
                    if (transform.position.x < minX)
                    {
                        MovingLeft = false;
                    }
                }
                else
                {
                    transform.position = transform.position.With(
                        x: transform.position.x + StrafeSpeed * Time.deltaTime);
                    if (transform.position.x > maxX)
                    {
                        MovingLeft = true;
                    }
                }
            }
            if (Aggro && !AttackCooldown.IsOnCooldown)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        Animator.SetTrigger("Attack");
        AttackCooldown.Use();
        var projectile = Instantiate(Projectile, transform.position, transform.rotation);
        var script = projectile.GetComponent<Projectile>();
        var dirToPlayer = (Player.transform.position - transform.position).Truncate();
        dirToPlayer.Normalize();
        script.Direction = dirToPlayer;
    }

    private void OnDamage()
    {
        Animator.SetTrigger("Hit");
    }

    private void OnDeath()
    {
        // Probably also want to spawn an effect or something
        Player.RefundJump();
        Destroy(gameObject);
    }
}
