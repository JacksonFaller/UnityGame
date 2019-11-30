using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] 
    private float _jumpForce = 50;

    [SerializeField]
    private float _wallJumpPowerMultiplier = 0.6f;

    [SerializeField]
    private ParticleSystem _jumpParticle;

    [SerializeField]
    private ParticleSystem _wallJumpParticle;

    public Vector2 DoJump(MovementContext context, Vector2 velocity)
    {
        if (Input.GetButtonDown(Configuration.Input.JumpButton))
        {
            context.AnimationScript.SetTrigger(Configuration.AnimatorParameters.JumpTrigger);

            if (context.Collision.OnGround)
                return GetJumpVelocity(Vector2.up, velocity);
            else if (context.Collision.OnWall)
                return WallJump(context, velocity);
        }

        return velocity;
    }

    private Vector2 WallJump(MovementContext context, Vector2 velocity)
    {
        if ((context.Side == 1 && context.Collision.OnRightWall) || (context.Side == -1 && context.Collision.OnLeftWall))
        {
            context.Side *= -1;
            context.AnimationScript.Flip(context.Side);
        }

        StartCoroutine(context.DisableMovement(.1f));

        Vector2 wallDir = context.Collision.OnRightWall ? Vector2.left : Vector2.right;
        Vector2 jumpDirection = Vector2.up * _wallJumpPowerMultiplier + wallDir * _wallJumpPowerMultiplier;
        context.IsWallJumped = true;
        return GetJumpVelocity(jumpDirection, velocity, true);
    }

    private Vector2 GetJumpVelocity(Vector2 dir, Vector2 velocity, bool wall = false)
    {
        //SlideParticle.transform.parent.localScale = new Vector3(GetParticleSide(), 1, 1);
        ParticleSystem particle = wall ? _wallJumpParticle : _jumpParticle;

        particle.Play();
        return new Vector2(velocity.x, 0) + dir * _jumpForce;
    }
}
