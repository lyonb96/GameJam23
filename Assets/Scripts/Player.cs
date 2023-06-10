using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum State
{
    Idle,
    Running,
    Attack,
}

public class Player : MonoBehaviour
{
    #region Components
    private Rigidbody2D RigidBody;

    private CapsuleCollider2D Collider;

    private SpriteRenderer Sprite;

    private Animator Animator;
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

    private State CurrentState;

    private bool IsGrounded;

    private bool CanJump => (IsGrounded || (!IsGrounded && !HasDoubleJumped)) && !IsAttacking;

    private float SlideStartTime;

    private bool IsSliding;

    private bool IsSlideJumping;

    private bool CanSlide => IsGrounded && !IsSliding && DesiredMove != 0.0F && !IsAttacking;

    private bool FacingLeft;
    #endregion

    #region Internal attack tracking
    private float LastAttackTime;

    private int StandardComboCounter;

    private int SlideJumpComboCounter;

    private bool CanAttack => !IsSliding && !IsAttacking;

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
            },
            new()
            {
                Name = "Attack2",
                RequiresPerfectTiming = false,
                PerfectAccuracy = 0.8F,
                AcceptableAccuracy = 0.4F,
                TimeWindow = 1F,
                AttackCenter = new Vector2(1, 0),
                AttackExtents = new Vector2(0.5F, 2.0F),
            },
            new()
            {
                Name = "Attack3",
                RequiresPerfectTiming = true,
                PerfectAccuracy = 0.7F,
                AcceptableAccuracy = 0.5F,
                TimeWindow = 1F,
                AttackCenter = new Vector2(0, 0),
                AttackExtents = new Vector2(2.5F, 1F),
            }
        }
    };
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        // Set default values
        MoveSpeed = 250.0F;
        JumpForce = 8.0F;
        DoubleJumpForce = 10.0F;
        SlideSpeed = 425.0F;
        SlideDuration = 0.5F;
    }

    private void FixedUpdate()
    {
        var groundHitTest = Physics2D.OverlapCapsuleAll(
            RigidBody.position + (Vector2.down * 0.01F) + Collider.offset,
            Collider.size * new Vector2(0.9F, 1.0F) * transform.localScale,
            Collider.direction,
            0F);
        var ground = groundHitTest.FirstOrDefault(x => x.gameObject != gameObject);
        if (ground == null)
        {
            IsGrounded = false;
        }
        else
        {
            if (!IsGrounded)
            {
                // Landed, add any logic here
                HasDoubleJumped = false;
                IsSlideJumping = false;
            }
            IsGrounded = true;
        }
        var y = RigidBody.velocity.y;
        if (WantsToJump && CanJump)
        {
            if (IsSliding)
            {
                IsSlideJumping = true;
            }
            StopSliding();
            WantsToJump = false;
            y = !IsGrounded
                ? DoubleJumpForce
                : JumpForce;
            Animator.SetTrigger("Jumping");
            if (!IsGrounded)
            {
                HasDoubleJumped = true;
            }
        }
        var lateralSpeed = (IsSliding || IsSlideJumping) ? SlideSpeed : MoveSpeed;
        var moveToApply = !IsSliding
            ? DesiredMove
            : FacingLeft
                ? -1.0F
                : 1.0F;
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
        Animator.SetBool("Running", DesiredMove != 0.0F);
        if (!IsSliding)
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
            IsSliding = true;
            SlideStartTime = Time.time;
            Animator.SetBool("Sliding", true);
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
    }

    void Attack()
    {
        // var anim = StandardComboCounter == 0
        //     ? "Attack1"
        //     : StandardComboCounter == 1 && (LastAttackTime + 0.75F) < Time.time
        //         ? "Attack2"
        //         : "Attack1";
        // LastAttackTime = Time.time;
        // Animator.SetTrigger(anim);
        // StandardComboCounter++;
        var anim = BasicAttackCombo.PlayAction();
        Animator.SetTrigger(anim.Name);
        IsAttacking = true;
        ActiveAttack = anim;
    }

    public void CheckForHit()
    {
        if (ActiveAttack == null)
        {
            return;
        }
        var start = new Vector2(transform.position.x, transform.position.y)
            + (ActiveAttack.AttackCenter * (FacingLeft ? -1.0F : 1.0F));
        var hits = Physics2D.OverlapBoxAll(start, ActiveAttack.AttackExtents, 0.0F);
        foreach (var hit in hits)
        {
            var healthComponent = hit.GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.Damage(1);
            }
        }
    }

    public void AttackFinish()
    {
        IsAttacking = false;
        ActiveAttack = null;
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
}