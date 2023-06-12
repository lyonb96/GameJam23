using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Player : MonoBehaviour
{
    #region Components
    private Rigidbody2D RigidBody;

    private CapsuleCollider2D Collider;

    private SpriteRenderer Sprite;

    private Animator Animator;

    private Health Health;

    public GameObject DeathScreen;
    #endregion

    #region Stats
    public float MoveSpeed;

    public float SlideSpeed;

    public float SlideDuration;

    public float Acceleration;

    public float JumpForce;

    public float DoubleJumpForce;
    #endregion

    #region Internal movement tracking
    private float DesiredMove;

    private bool WantsToJump;

    private bool HasDoubleJumped;

    private bool IsJumping;

    private bool IsGrounded;

    private bool CanJump => (IsGrounded || (!IsGrounded && !HasDoubleJumped && CanDoubleJump)) && !IsAttacking && !PausedMovement && RigidBody.simulated;

    private bool CanDoubleJump;

    private float SlideStartTime;

    private bool IsSliding;

    private bool IsSlideJumping;

    private bool CanSlide => IsGrounded && !IsSliding && DesiredMove != 0.0F && !IsAttacking && !PausedMovement && RigidBody.simulated;

    private bool FacingLeft;

    private Vector3 CurrentCheckpoint;

    public bool IsRunningRight => DesiredMove > 0.0F;

    private bool PausedMovement;

    private bool WaitingForRespawn;
    #endregion

    #region Internal attack tracking
    private bool CanAttack => !IsSliding && !IsAttacking && !PausedMovement;

    private bool IsAttacking;

    private ComboAction ActiveAttack;

    private ComboHelper BasicAttackCombo = new()
    {
        Combos = new()
        {
            new()
            {
                Name = "Attack1",
                RequiresPerfectTiming = false,
                PerfectAccuracy = 0.8F,
                AcceptableAccuracy = 0.6F,
                TimeWindow = 0.0F,
                AttackCenter = new Vector2(1, 0),
                AttackExtents = new Vector2(0.5F, 2.0F),
                Damage = 1,
            },
            new()
            {
                Name = "Attack2",
                RequiresPerfectTiming = false,
                PerfectAccuracy = 0.8F,
                AcceptableAccuracy = 0.25F,
                TimeWindow = 1F,
                AttackCenter = new Vector2(1, 0),
                AttackExtents = new Vector2(0.5F, 2.0F),
                Damage = 2,
            },
            new()
            {
                Name = "Attack3",
                RequiresPerfectTiming = true,
                PerfectAccuracy = 0.65F,
                AcceptableAccuracy = 0.4F,
                TimeWindow = 1F,
                AttackCenter = new Vector2(0, 0),
                AttackExtents = new Vector2(2.5F, 1F),
                Damage = 4,
            }
        }
    };
    #endregion

    #region Audio
    public AudioClip Hit1, Hit2, Hit3, Swing, BigSwing, Damage, SlideSound;

    private AudioSource SwingSource;
    private AudioSource HitSource;
    private AudioSource DamageSource;
    private AudioSource SlideSource;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CurrentCheckpoint = transform.position;
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        Health = GetComponent<Health>();
        var audioSources = GetComponents<AudioSource>();
        SwingSource = audioSources[0];
        HitSource = audioSources[1];
        DamageSource = audioSources[2];
        SlideSource = audioSources[3];
        Health.OnDamage.AddListener(OnDamage);
        Health.OnDeath.AddListener(OnDeath);
        // Set default values
        MoveSpeed = 250.0F;
        JumpForce = 10.0F;
        DoubleJumpForce = 10.0F;
        SlideSpeed = 425.0F;
        SlideDuration = 0.5F;
    }

    private void FixedUpdate()
    {
        var downOffset = IsSliding ? 0.05F : 0.02F;
        var groundHitTest = Physics2D.OverlapCapsuleAll(
            Collider.transform.position.Truncate() + (Vector2.down * downOffset) + Collider.offset,
            Collider.size * new Vector2(0.95F, 1.0F) * transform.localScale,
            Collider.direction,
            0F);
        var ground = groundHitTest.FirstOrDefault(x => x.gameObject != gameObject && !x.isTrigger);
        var antiStick = false;
        if (ground == null)
        {
            IsGrounded = false;
            // Check if we need to prevent sideways movement
            var sideHitTest = Physics2D.OverlapCapsuleAll(
                RigidBody.position + Collider.offset,
                Collider.size,
                Collider.direction,
                0.0F);
            if (sideHitTest.Any(x => x.gameObject != gameObject && !x.isTrigger))
            {
                antiStick = true;
            }
        }
        else
        {
            if (!IsGrounded)
            {
                // Landed, add any logic here
                HasDoubleJumped = false;
                IsSlideJumping = false;
            }
            CanDoubleJump = false;
            IsGrounded = true;
        }
        var y = RigidBody.velocity.y;
        if (WantsToJump && CanJump)
        {
            if (IsSliding)
            {
                IsSlideJumping = true;
                StopSliding();
            }
            WantsToJump = false;
            y = !IsGrounded
                ? DoubleJumpForce
                : JumpForce;
            Animator.SetTrigger("Jumping");
            IsJumping = true;
            if (!IsGrounded)
            {
                HasDoubleJumped = true;
            }
        }
        Animator.SetBool("Falling", !IsGrounded && !IsJumping && !IsAttacking);
        var lateralSpeed = (IsSliding || IsSlideJumping) ? SlideSpeed : MoveSpeed;
        var moveToApply = !IsSliding
            ? DesiredMove
            : FacingLeft
                ? -1.0F
                : 1.0F;
        if (antiStick)
        {
            moveToApply = 0.0F;
        }
        RigidBody.velocity = new(moveToApply * Time.fixedDeltaTime * lateralSpeed, y);
        if (IsSliding && (SlideStartTime + SlideDuration) <= Time.time)
        {
            StopSliding();
        }
    }

    // Update is called once per frame
    void Update()
    {
        DesiredMove = Input.GetAxis("Horizontal");
        if (PausedMovement)
        {
            DesiredMove = 0.0F;
        }
        Animator.SetBool("Running", DesiredMove != 0.0F);
        if (!IsSliding && !IsAttacking)
        {
            if (DesiredMove < 0.0F)
            {
                Sprite.flipX = true;
                FacingLeft = true;
            }
            if (DesiredMove > 0.0F)
            {
                Sprite.flipX = false;
                FacingLeft = false;
            }
        }
        if (Input.GetButtonDown("Jump") && CanJump)
        {
            WantsToJump = true;
        }
        if (Input.GetButtonDown("Slide") && CanSlide)
        {
            var accuracy = RhythmManager.GetInstance().GetBeatAccuracy(0.85F, 0.5F);
            if (accuracy == BeatAccuracy.Miss)
            {
                // Missed the slide, show some kind of message?
            }
            else
            {
                SlideSource.clip = SlideSound;
                SlideSource.Play();
                IsSliding = true;
                SlideStartTime = Time.time;
                Animator.SetBool("Sliding", true);
                var height = Collider.size.y;
                var width = Collider.size.x;
                var off = Collider.offset.y;
                Collider.size = new Vector2(width * 0.9F, height * 0.5F);
                Collider.offset = new Vector2(Collider.offset.x, off - (height * 0.25F));
            }
        }
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     var beat = RhythmManager.GetInstance().GetBeatAccuracy(0.9F, 0.8F);
        //     Debug.Log(beat.ToString());
        // }
        if (Input.GetButtonDown("Attack") && CanAttack)
        {
            Attack();
        }
    }

    void StopSliding()
    {
        IsSliding = false;
        Animator.SetBool("Sliding", false);
        var height = Collider.size.y;
        var width = Collider.size.x;
        var off = Collider.offset.y;
        Collider.size = new Vector2(width / 0.9F, height * 2F);
        Collider.offset = new Vector2(Collider.offset.x, off + (height * 0.5F));
    }

    void Attack()
    {
        SwingSource.clip = Swing;
        if (IsGrounded)
        {
            var anim = BasicAttackCombo.PlayAction();
            Animator.SetTrigger(anim.Name);
            ActiveAttack = anim;
            if (anim.Name == "Attack3")
            {
                SwingSource.clip = BigSwing;
            }
        }
        else
        {
            ActiveAttack = new()
            {
                Name = "AirAttack1",
                Damage = 2,
                AttackCenter = new Vector2(),
                AttackExtents = new Vector2(2.5F, 1.0F),
                TimeWindow = 0.0F,
            };
            Animator.SetTrigger(ActiveAttack.Name);
        }
        Animator.SetBool("Falling", false);
        IsAttacking = true;
        SwingSource.Play();
    }

    public void CheckForHit()
    {
        if (ActiveAttack == null)
        {
            return;
        }
        var start = new Vector2(transform.position.x, transform.position.y)
            + (ActiveAttack.AttackCenter * (FacingLeft ? -1.0F : 1.0F));
        var hits = Extensions.CheckForHits(start, ActiveAttack.AttackExtents, ignores: new[] { gameObject });
        var success = false;
        foreach (var hit in hits)
        {
            var healthComponent = hit.GetComponent<Health>();
            healthComponent.Damage(ActiveAttack.Damage);
            success = true;
        }
        if (success)
        {
            HitSource.clip = ActiveAttack.Name switch
            {
                "Attack1" => Hit1,
                "Attack2" => Hit2,
                "Attack3" => Hit3,
                _ => Hit1,
            };
            HitSource.Play();
        }
    }

    public void AttackFinish()
    {
        IsAttacking = false;
        ActiveAttack = null;
    }

    public void JumpFinished()
    {
        IsJumping = false;
    }

    public void SetCheckpoint(Vector3 checkpoint)
    {
        CurrentCheckpoint = checkpoint;
    }

    public void FellOutOfWorld()
    {
        if (WaitingForRespawn)
        {
            return;
        }
        RigidBody.simulated = false;
        Health.Damage(1);
        if (Health.CurrentHealth > 0)
        {
            StartCoroutine(WaitForRespawn());
        }
    }

    public void PauseMovement()
    {
        PausedMovement = true;
    }

    public void ResumeMovement()
    {
        PausedMovement = false;
    }

    public void RefundJump()
    {
        CanDoubleJump = true;
        HasDoubleJumped = false;
    }

    private IEnumerator WaitForRespawn()
    {
        WaitingForRespawn = true;
        yield return new WaitForSeconds(2);
        transform.position = CurrentCheckpoint;
        RigidBody.simulated = true;
        WaitingForRespawn = false;
    }

    private void OnDamage()
    {
        // Play damage sound?
        HitSource.clip = Damage;
        HitSource.Play();
    }

    private void OnDeath()
    {
        PauseMovement();
        Animator.SetTrigger("Death");
        var canvas = GameObject.Find("WorldCanvas");
        Instantiate(DeathScreen, canvas.transform);
        // var script = deathscreen.GetComponent<DeathScreenScript>();
        // var currentScene = SceneManager.GetActiveScene().name;
        // SceneManager.LoadScene(currentScene);
    }
}

class ComboHelper
{
    public List<ComboAction> Combos;

    private int ComboCounter;

    private float LastTime;

    public ComboAction PlayAction()
    {
        // Check if the Combo Counter needs to be reset
        if (ComboCounter >= Combos.Count
            || (ComboCounter > 0 && LastTime + Combos[ComboCounter].TimeWindow < Time.time))
        {
            ComboCounter = 0;
        }
        var comboStepToUse = Combos[ComboCounter];
        var accuracy = RhythmManager.GetInstance().GetBeatAccuracy(comboStepToUse.PerfectAccuracy, comboStepToUse.AcceptableAccuracy);
        if (comboStepToUse.RequiresPerfectTiming && accuracy != BeatAccuracy.Perfect)
        {
            ComboCounter = 0;
            comboStepToUse = Combos[ComboCounter];
        }
        else if (accuracy != BeatAccuracy.Miss)
        {
            ComboCounter++;
        }
        else
        {
            ComboCounter = 0;
            comboStepToUse = Combos[ComboCounter];
        }
        LastTime = Time.time;
        return comboStepToUse;
    }
}

class ComboAction
{
    public string Name;

    public bool RequiresPerfectTiming;

    public float TimeWindow;

    public float PerfectAccuracy;

    public float AcceptableAccuracy;

    public Vector2 AttackCenter;

    public Vector2 AttackExtents;

    public int Damage;
}