using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlide : MonoBehaviour
{
    [SerializeField]
    private LedgeBounce _ledgeBounce;

    [SerializeField]
    private float _slideSpeed = 2f;

    [SerializeField]
    private ParticleSystem slideParticle;


    public Vector2 DoWallSlide(float x, Vector2 velocity, MovementContext context)
    {
        if (context.Collision.OnWall && !context.Collision.OnGround && !_ledgeBounce.IsLedgeBouncing)
        {
            if (context.Collision.OnRightWall && x > 0 || context.Collision.OnLeftWall && x < 0)
            {
                context.IsWallSliding = true;
                velocity = GetWallSlideVelocity(velocity, context);
            }
        }
        else
        {
            context.IsWallSliding = false;
        }
        WallParticle(context);
        return velocity;
    }

    private Vector2 GetWallSlideVelocity(Vector2 velocity, MovementContext context)
    {
        if (context.Collision.WallSide != context.Side)
            context.AnimationScript.Flip(context.Side * -1);

        if (!context.CanMove)
            return velocity;

        float slideSpeed = velocity.y > 0 ? velocity.y : -_slideSpeed;
        return new Vector2(0, slideSpeed);
    }

    private void WallParticle(MovementContext context)
    {
        var main = slideParticle.main;
        if (context.IsWallSliding)
        {
            slideParticle.transform.parent.localScale = new Vector3(context.Collision.WallSide * -1, 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }
}
