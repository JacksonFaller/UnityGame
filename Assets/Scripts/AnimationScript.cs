using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimationScript : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Movement _movement;

    [SerializeField]
    private Collision _collision = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movement = GetComponentInParent<Movement>();
    }

    void Update()
    {
        _animator.SetBool("onGround", _collision.OnGround);
        _animator.SetBool("onWall", _collision.OnWall);
        _animator.SetBool("onRightWall", _collision.OnRightWall);
        _animator.SetBool("wallSlide", _movement.Context.IsWallSliding);
        _animator.SetBool("canMove", _movement.Context.CanMove);
        _animator.SetBool("isDashing", _movement.Context.IsDashing);
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

        if (_movement.Context.IsWallSliding && 
            ((side == -1 && _spriteRenderer.flipX) || (side == 1 && !_spriteRenderer.flipX)))
            return;

        _spriteRenderer.flipX = side != 1;
    }

    public int Side => _spriteRenderer.flipX ? -1 : 1;
}
