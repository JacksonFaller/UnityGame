using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BetterJumping))]
[RequireComponent(typeof(Collision))]
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D Rigidbody;
    private Collision _collision;
    private AnimationScript _animation;
    private BetterJumping _betterJumping;

    private bool _isGrounded;
    private bool _hasDashed;

    [Space]
    [Header("Stats")]
    public float Speed = 10;
    public float JumpForce = 50;
    public float SlideSpeed = 5;
    public float WallJumpLerp = 10;
    public float DashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool CanMove;
    public bool IsWallGrabbing;
    public bool IsWallJumped;
    public bool IsWallSliding;
    public bool IsDashing;

    [Space]
    public int Side = 1;

    [Space]
    [Header("Particles")]
    public ParticleSystem DashParticle;
    public ParticleSystem JumpParticle;
    public ParticleSystem WallJumpParticle;
    public ParticleSystem SlideParticle;

    [Space]
    public GhostTrail GhostTrail;
    public RippleEffect RippleEffect;

    // Start is called before the first frame update
    void Start()
    {
        _collision = GetComponent<Collision>();
        Rigidbody = GetComponent<Rigidbody2D>();
        _animation = GetComponentInChildren<AnimationScript>();
        _betterJumping = GetComponent<BetterJumping>();

        if (GhostTrail == null) Debug.LogError("GhostTrail is not referenced!");
        if (RippleEffect == null) Debug.LogError("RippleEffect is not referenced!");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        _animation.SetHorizontalMovement(x, y, Rigidbody.velocity.y);

        if (_collision.onWall && Input.GetButton("Fire3") && CanMove)
        {
            if (Side != _collision.wallSide)
            {
                Side *= -1;
                _animation.Flip(Side);
            }

            IsWallGrabbing = true;
            IsWallSliding = false;
        }

        if (Input.GetButtonUp("Fire3") || !_collision.onWall || !CanMove)
        {
            IsWallGrabbing = false;
            IsWallSliding = false;
        }

        if (_collision.onGround && !IsDashing)
        {
            IsWallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        /// forsenWhat ???? 
        if (IsWallGrabbing && !IsDashing)
        {
            Rigidbody.gravityScale = 0;
            if (x > .2f || x < -.2f)
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, y * (Speed * speedModifier));
        }
        else
        {
            Rigidbody.gravityScale = 3;
        }

        if (_collision.onWall && !_collision.onGround)
        {
            if (((_collision.onRightWall && x > 0) || (_collision.onLeftWall && x < 0)) && !IsWallGrabbing)
            {
                IsWallSliding = true;
                WallSlide();
            }
        }

        if (!_collision.onWall || _collision.onGround)
            IsWallSliding = false;

        if (Input.GetButtonDown("Jump"))
        {
            _animation.SetTrigger("jump");

            if (_collision.onGround)
                Jump(Vector2.up, false);
            if (_collision.onWall && !_collision.onGround)
                WallJump();
        }

        if (Input.GetButtonDown("Fire1") && !_hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
            else
                Dash(Side, 0);
        }

        if (_collision.onGround && !_isGrounded)
        {
            GroundTouch();
            _isGrounded = true;
        }

        if (!_collision.onGround && _isGrounded)
        {
            _isGrounded = false;
        }

        WallParticle(y);

        if (IsWallGrabbing || IsWallSliding || !CanMove)
            return;

        if (x > 0)
        {
            Side = 1;
            _animation.Flip(Side);
        }
        if (x < 0)
        {
            Side = -1;
            _animation.Flip(Side);
        }
    }

    void GroundTouch()
    {
        _hasDashed = false;
        IsDashing = false;

        Side = _animation.SpriteRenderer.flipX ? -1 : 1;

        JumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        RippleEffect.Emit(Camera.main.WorldToViewportPoint(transform.position));

        _hasDashed = true;

        _animation.SetTrigger("dash");

        Rigidbody.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        Rigidbody.velocity += dir.normalized * DashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        GhostTrail.ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, (float x) => Rigidbody.drag = x);

        DashParticle.Play();
        Rigidbody.gravityScale = 0;
        _betterJumping.enabled = false;
        IsWallJumped = true;
        IsDashing = true;

        yield return new WaitForSeconds(.3f);

        DashParticle.Stop();
        Rigidbody.gravityScale = 3;
        _betterJumping.enabled = true;
        IsWallJumped = false;
        IsDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (_collision.onGround)
            _hasDashed = false;
    }

    private void WallJump()
    {
        if ((Side == 1 && _collision.onRightWall) || Side == -1 && _collision.onLeftWall)
        {
            Side *= -1;
            _animation.Flip(Side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = _collision.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        IsWallJumped = true;
    }

    private void WallSlide()
    {
        if (_collision.wallSide != Side)
            _animation.Flip(Side * -1);

        if (!CanMove)
            return;

        bool pushingWall = false;
        if ((Rigidbody.velocity.x > 0 && _collision.onRightWall) || (Rigidbody.velocity.x < 0 && _collision.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : Rigidbody.velocity.x;

        Rigidbody.velocity = new Vector2(push, -SlideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!CanMove || IsWallGrabbing) return;

        if (IsWallJumped)
            Rigidbody.velocity = Vector2.Lerp(Rigidbody.velocity, 
                new Vector2(dir.x * Speed, Rigidbody.velocity.y), WallJumpLerp * Time.deltaTime);
        else
            Rigidbody.velocity = new Vector2(dir.x * Speed, Rigidbody.velocity.y);
    }

    private void Jump(Vector2 dir, bool wall)
    {
        SlideParticle.transform.parent.localScale = new Vector3(GetParticleSide(), 1, 1);
        ParticleSystem particle = wall ? WallJumpParticle : JumpParticle;

        Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
        Rigidbody.velocity += dir * JumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        CanMove = false;
        yield return new WaitForSeconds(time);
        CanMove = true;
    }

    void WallParticle(float vertical)
    {
        var main = SlideParticle.main;

        if (IsWallSliding || (IsWallGrabbing && vertical < 0))
        {
            SlideParticle.transform.parent.localScale = new Vector3(GetParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int GetParticleSide() => _collision.wallSide * -1;
}
