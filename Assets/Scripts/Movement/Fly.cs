using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    [SerializeField]
    private float _flySpeed = 10f;

    [SerializeField]
    private float _fallSpeed = 0.5f;

    public Vector2 GetFlyVelocity(float x, MovementContext context, Vector2 velocity)
    {
        if (Input.GetButton(Configuration.Input.FlyButton))
        {
            if (context.Collision.OnWall || context.Collision.OnGround)
            {
                context.GravityScale = 3f;
                return velocity;
            }

            context.GravityScale = 0f;
            float flySpeed = context.Side == 1 ? _flySpeed : -_flySpeed;
            if (x != 0 && context.CanMove)
            {
                flySpeed = x > 0 ? _flySpeed : -_flySpeed;
                if ((x < 0 && context.Side == 1) || (x > 0 && context.Side == -1))
                    StartCoroutine(context.DisableAirMovement(0.4f));
            }

            return new Vector2(flySpeed, -_fallSpeed);
        }

        if (Input.GetButtonUp(Configuration.Input.FlyButton))
        {
            context.GravityScale = 3f;
        }

        return velocity;
    }
}
