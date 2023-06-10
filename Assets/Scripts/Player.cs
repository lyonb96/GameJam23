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

    private bool CanJump => IsGrounded || (!IsGrounded && !HasDoubleJumped);

    private float SlideStartTime;

    private bool IsSliding;

    private bool CanSlide => IsGrounded && !IsSliding;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        // Set default values
        MoveSpeed = 150.0F;
        JumpForce = 4.0F;
        DoubleJumpForce = 6.0F;
        SlideSpeed = 250.0F;
        SlideDuration = 0.5F;
    }

    private void FixedUpdate()
    {
        var jumping = false;
        var groundHitTest = Physics2D.OverlapCapsuleAll(
            RigidBody.position + (Vector2.down * 0.01F),
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
            }
            IsGrounded = true;
        }
        var y = RigidBody.velocity.y;
        if (WantsToJump && CanJump)
        {
            StopSliding();
            WantsToJump = false;
            y = !IsGrounded
                ? DoubleJumpForce
                : JumpForce;
            jumping = true;
            if (!IsGrounded)
            {
                HasDoubleJumped = true;
            }
        }
        Animator.SetBool("Jumping", jumping);
        var lateralSpeed = IsSliding ? SlideSpeed : MoveSpeed;
        RigidBody.velocity = new(DesiredMove * Time.fixedDeltaTime * lateralSpeed, y);
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
        if (DesiredMove < 0.0F)
        {
            Sprite.flipX = true;
        }
        if (DesiredMove > 0.0F)
        {
            Sprite.flipX = false;
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var beat = RhythmManager.GetInstance().GetBeatAccuracy(0.9F, 0.8F);
            Debug.Log(beat.ToString());
        }
    }

    void StopSliding()
    {
        IsSliding = false;
        Animator.SetBool("Sliding", false);
    }
}
