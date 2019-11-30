using System.Collections;
using UnityEngine;

public class MovementContext
{
    public bool CanMove { get; set; } = true;
    public bool IsWallJumped { get; set; }
    public bool IsWallSliding { get; set; }
    public bool IsDashing { get; set; }
    public bool HasDashed { get; set; }
    public float GravityScale { get; set; }
    public float Drag { get; set; }

    public int Side { get; set; } = 1;

    public Collision Collision { get; }

    public AnimationScript AnimationScript { get; }

    public MovementContext(Collision collision, AnimationScript animationScript)
    {
        Collision = collision;
        AnimationScript = animationScript;
    }

    public IEnumerator DisableMovement(float time)
    {
        CanMove = false;
        yield return new WaitForSeconds(time);
        CanMove = true;
    }

    public IEnumerator DisableAirMovement(float time)
    {
        yield return new WaitForEndOfFrame();
        yield return DisableMovement(time);
    }
}