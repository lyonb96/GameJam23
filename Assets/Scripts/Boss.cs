using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float MoveSpeed = 400.0F;

    public bool Pause;

    private Health Health;

    private Player Player;

    private Animator Animator;

    private SpriteRenderer Sprite;

    private int Phase;

    private bool Attacking;

    private float MoveDirection;

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
        transform.position += new Vector3(MoveDirection * MoveSpeed * Time.deltaTime, 0.0F, 0.0F);
    }

    IEnumerator RunAttack1()
    {
        Attacking = true;
        // Step 1: Run away from the player
        var playerIsLeft = Player.transform.position.x < transform.position.x;
        MoveDirection = playerIsLeft ? 1.0F : -1.0F;
        yield return new WaitForSeconds(2.0F);
        MoveDirection = 0.0F;
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
        var playerIsLeft = Player.transform.position.x < transform.position.x;
        MoveDirection = playerIsLeft ? 1.0F : -1.0F;
        yield return new WaitForSeconds(2.0F);
        MoveDirection = 0.0F;
        // Step 2: Raise the sword
        Animator.SetBool("Raise", true);
        yield return new WaitForSeconds(2.0F);
        Animator.SetBool("Raise", false);
        // Step 3: Idle for a bit
        yield return new WaitForSeconds(3.0F);
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
        if (Phase == 0 && Health.CurrentHealthPercent <= 0.66F)
        {
            Phase++;
        }
        else if (Phase == 1 && Health.CurrentHealthPercent <= 0.33F)
        {
            Phase++;
        }
    }

    void OnDeath()
    {
        // Notify the level script
    }
}
