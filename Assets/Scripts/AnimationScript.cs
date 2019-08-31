using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimationScript : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer SpriteRenderer;

    private Animator _animator;
    private Movement _movement;
    private Collision _collision;

    void Start()
    {
        _animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _collision = GetComponentInParent<Collision>();
        _movement = GetComponentInParent<Movement>();
    }

    void Update()
    {
        _animator.SetBool("onGround", _collision.OnGround);
        _animator.SetBool("onWall", _collision.OnWall);
        _animator.SetBool("onRightWall", _collision.OnRightWall);
        _animator.SetBool("wallGrab", _movement.IsWallGrabbing);
        _animator.SetBool("wallSlide", _movement.IsWallSliding);
        _animator.SetBool("canMove", _movement.CanMove);
        _animator.SetBool("isDashing", _movement.IsDashing);
    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        _animator.SetFloat("HorizontalAxis", x);
        _animator.SetFloat("VerticalAxis", y);
        _animator.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        _animator.SetTrigger(trigger);
    }

    public void Flip(int side)
    {

        if (_movement.IsWallGrabbing || _movement.IsWallSliding)
        {
            if (side == -1 && SpriteRenderer.flipX)
                return;

            if (side == 1 && !SpriteRenderer.flipX)
            {
                return;
            }
        }

        bool state = side != 1;
        SpriteRenderer.flipX = state;
    }
}
