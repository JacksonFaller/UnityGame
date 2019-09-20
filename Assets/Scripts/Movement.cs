﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(BetterJumping))]
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private AnimationScript _animationScript;
    private BetterJumping _betterJumping;

    private bool _isGrounded;
    private bool _hasDashed;
    private int _side = 1;
    private RunState _runState = RunState.Deceleration;


    public bool CanMove { get; private set; } = true;
    public bool IsWallGrabbing { get; private set; }
    public bool IsWallJumped { get; private set; }
    public bool IsWallSliding { get; private set; }
    public bool IsDashing { get; private set; }

    #region Inspector values

    [Space]
    [Header("Stats")]
    public float RunSpeed = 20f;
    public float AccelerationSpeed = 5f;
    public float DecelerationnSpeed = 10f;

    public float JumpForce = 50;
    public float SlideSpeed = 5;
    public float WallJumpLerp = 10;
    public float DashSpeed = 20;
    public float WallJumpPowerMultiplier = 0.6f;
    public float WallClimbSpeedMultiplier = 0.5f;
    public float FlySpeed = 10f;
    public float FallSpeed = 0.25f;

    [Space]
    [Header("Particles")]
    public ParticleSystem DashParticle;
    public ParticleSystem JumpParticle;
    public ParticleSystem WallJumpParticle;
    public ParticleSystem SlideParticle;

    [Space]
    [Header("References")]
    public GhostTrail GhostTrail;
    public RippleEffect RippleEffect;

    [SerializeField]
    private LedgeBounce _ledgeBounce = null;

    [SerializeField]
    private Collision _collision = null;



    #endregion

    private Action _onHorizontalAxis;
    private bool _horizontalAxisState;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animationScript = GetComponentInChildren<AnimationScript>();
        _betterJumping = GetComponent<BetterJumping>();

        if (GhostTrail == null) Debug.LogError("GhostTrail is not referenced!");
        if (RippleEffect == null) Debug.LogError("RippleEffect is not referenced!");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis(Configuration.Input.HorizontalAxis);
        float y = Input.GetAxis(Configuration.Input.VerticalAxis);

        Walk(x);
        _animationScript.SetHorizontalMovement(x, y, _rigidbody.velocity.y);
        DoGrabWall();

        if (Input.GetButtonUp(Configuration.Input.WallClimbButton) || !_collision.OnWall || !CanMove)
            IsWallGrabbing = IsWallSliding = false;

        if (_collision.OnGround && !IsDashing)
            IsWallJumped = false;

        UpdateGravityScale(y);
        Fly(x);
        DoWallSlide(x);
        DoJump();
        DoDash();
        UpdateIsGrounded();
        WallParticle(y);

        if (!IsWallGrabbing && !IsWallSliding && CanMove && x != 0)
        {
            _side = x > 0 ? 1 : -1;
            _animationScript.Flip(_side);
        }
    }


    #region Dash

    private void Dash(Vector2 direction)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .25f, 14, 90, false, true);
        RippleEffect.Emit(Camera.main.WorldToViewportPoint(transform.position));

        _hasDashed = true;
        _animationScript.SetTrigger(Configuration.AnimatorParameters.DashTrigger);

        _rigidbody.velocity = direction.normalized * DashSpeed;
        StartCoroutine(DashWait());
    }

    private IEnumerator DashWait()
    {
        GhostTrail.ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(15, 0, 0.35f, (float x) => _rigidbody.drag = x);

        DashParticle.Play();
        UpdateDashState(0, false);

        yield return new WaitForSeconds(.3f);

        DashParticle.Stop();
        UpdateDashState(3, true);
    }

    private void UpdateDashState(float gravityScale, bool jumpingEnabled)
    {
        _rigidbody.gravityScale = gravityScale;
        _betterJumping.enabled = jumpingEnabled;
        IsWallJumped = !jumpingEnabled;
        IsDashing = !jumpingEnabled;
    }

    private IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (_collision.OnGround)
            _hasDashed = false;
    }

    #endregion

    #region Wall

    private void WallJump()
    {
        if ((_side == 1 && _collision.OnRightWall) || (_side == -1 && _collision.OnLeftWall))
        {
            _side *= -1;
            _animationScript.Flip(_side);
        }

        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = _collision.OnRightWall ? Vector2.left : Vector2.right;
        Vector2 jumpDirection = Vector2.up * WallJumpPowerMultiplier + wallDir * WallJumpPowerMultiplier;
        Jump(jumpDirection, true);
        IsWallJumped = true;
    }

    private void WallSlide()
    {
        if (_collision.WallSide != _side)
            _animationScript.Flip(_side * -1);

        if (!CanMove)
            return;

        float slideSpeed = _rigidbody.velocity.y > 0 ? _rigidbody.velocity.y : -SlideSpeed;
        _rigidbody.velocity = new Vector2(0, slideSpeed);
    }

    #endregion

    private void Walk(float x)
    {
        if (!CanMove || IsWallGrabbing) return;

        if (IsWallJumped)
        {
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity,
               new Vector2(x * RunSpeed / 2, _rigidbody.velocity.y), WallJumpLerp * Time.deltaTime);
        }
        else if (_collision.OnGround)
        {
            float horizontalAxisRaw = Input.GetAxisRaw(Configuration.Input.HorizontalAxis);
            switch (_runState)
            {
                case RunState.Run:
                {
                    // TODO: Need to thing about gamepad controls where input value can be less than 1 (0 -> 1, -1 -> 0)
                    if (horizontalAxisRaw == 0 || (horizontalAxisRaw < 0 && _rigidbody.velocity.x > 0) || (horizontalAxisRaw > 0 && _rigidbody.velocity.x < 0))
                    {
                        _runState = RunState.Deceleration;
                    }
                    else
                    {
                        _rigidbody.velocity = new Vector2(RunSpeed * x, _rigidbody.velocity.y);
                    }
                    break;
                }
                case RunState.Acceleration:
                {
                    if (horizontalAxisRaw == 0 || (x < 0 && _rigidbody.velocity.x > 0) || (x > 0 && _rigidbody.velocity.x < 0))
                    {
                        _runState = RunState.Deceleration;
                    }
                    else
                    {
                        if (Mathf.Abs(_rigidbody.velocity.x) == RunSpeed)
                        {
                            _runState = RunState.Run;
                        }
                        else
                        {
                            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity,
                                new Vector2(RunSpeed * x, _rigidbody.velocity.y), Time.deltaTime * AccelerationSpeed);
                        }
                    }
                    break;
                }
                case RunState.Deceleration:
                {
                    if ((horizontalAxisRaw > 0 && _rigidbody.velocity.x >= 0) || (horizontalAxisRaw < 0 && _rigidbody.velocity.x <= 0))
                    {
                        _runState = RunState.Acceleration;
                    }
                    else
                    {
                        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity,
                            new Vector2(0, _rigidbody.velocity.y), Time.deltaTime * DecelerationnSpeed);
                    }
                    break;
                }
            }
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        SlideParticle.transform.parent.localScale = new Vector3(GetParticleSide(), 1, 1);
        ParticleSystem particle = wall ? WallJumpParticle : JumpParticle;

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.velocity += dir * JumpForce;

        particle.Play();
    }

    private void Fly(float x)
    {
        if (Input.GetButton(Configuration.Input.FlyButton))
        {
            if (_collision.OnWall || _isGrounded) return;

            _rigidbody.gravityScale = 0f;
            float flySpeed = _side == 1 ? FlySpeed : -FlySpeed;
            if (x != 0 && CanMove)
            {
                flySpeed = x > 0 ? FlySpeed : -FlySpeed;
                if ((x < 0 && _side == 1) || (x > 0 && _side == -1))
                    StartCoroutine(DisableAirMovement(0.4f));
            }

            _rigidbody.velocity = new Vector2(flySpeed, -FallSpeed);
        }
        else if (Input.GetButtonUp(Configuration.Input.FlyButton))
        {
            _rigidbody.gravityScale = 3f;
        }
    }

    #region Corroutines



    private IEnumerator DisableMovement(float time)
    {
        CanMove = false;
        yield return new WaitForSeconds(time);
        CanMove = true;
    }

    private IEnumerator DisableAirMovement(float time)
    {
        yield return new WaitForEndOfFrame();
        yield return DisableMovement(time);
    }

    #endregion

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

    private int GetParticleSide() => _collision.WallSide * -1;

    private void UpdateIsGrounded()
    {
        if (_collision.OnGround)
        {
            if (_isGrounded) return;
            JumpParticle.Play();
            _side = _animationScript.Side;
            _hasDashed = false;
            IsDashing = false;
            _isGrounded = true;
        }
        else if (_isGrounded)
        {
            _isGrounded = false;
        }
    }

    private void DoJump()
    {
        if (Input.GetButtonDown(Configuration.Input.JumpButton))
        {
            _animationScript.SetTrigger(Configuration.AnimatorParameters.JumpTrigger);

            if (_collision.OnGround)
                Jump(Vector2.up, false);
            else if (_collision.OnWall)
                WallJump();
        }
    }

    private void DoDash()
    {
        if (Input.GetButtonDown(Configuration.Input.DashButton) && !_hasDashed)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;
            Dash(direction);
        }
    }

    private void DoGrabWall()
    {
        if (_collision.OnWall && Input.GetButton(Configuration.Input.WallClimbButton) && CanMove)
        {
            if (_side != _collision.WallSide)
            {
                _side *= -1;
                _animationScript.Flip(_side);
            }
            IsWallGrabbing = true;
            IsWallSliding = false;
        }
    }

    private void UpdateGravityScale(float y)
    {

        if (IsWallGrabbing && !IsDashing)
        {
            _rigidbody.gravityScale = 0;
            float speedModifier = y > 0 ? WallClimbSpeedMultiplier : 1;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, y * (RunSpeed * speedModifier));
        }
        else
        {
            _rigidbody.gravityScale = 3;
        }
    }

    private void DoWallSlide(float x)
    {
        if (_collision.OnWall && !_collision.OnGround && !_ledgeBounce.IsLedgeBouncing)
        {
            if (((_collision.OnRightWall && x > 0) || (_collision.OnLeftWall && x < 0)) && !IsWallGrabbing)
            {
                IsWallSliding = true;
                WallSlide();
            }
        }
        else
        {
            IsWallSliding = false;
        }
    }
}


public enum RunState
{
    Acceleration,
    Deceleration,
    Run
}