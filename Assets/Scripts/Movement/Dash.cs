using DG.Tweening;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(BetterJumping))]
public class Dash : MonoBehaviour
{
    [SerializeField]
    private RippleEffect _rippleEffect;

    [SerializeField]
    private GhostTrail _ghostTrail;

    [SerializeField]
    private float _dashSpeed = 20;

    [SerializeField]
    private ParticleSystem _dashParticle;

    private BetterJumping _betterJumping;
    private AnimationScript _animationScript;

    public void Start()
    {
        _animationScript = GetComponentInChildren<AnimationScript>();
        _betterJumping = GetComponent<BetterJumping>();
    }

    public Vector2 DoDash(Vector2 veloctiy, MovementContext context)
    {
        if (Input.GetButtonDown(Configuration.Input.DashButton) && !context.HasDashed)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;
            return GetDashVelocity(direction, context);
        }
        return veloctiy;
    }

    private Vector2 GetDashVelocity(Vector2 direction, MovementContext context)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .25f, 14, 90, false, true);
        _rippleEffect.Emit(Camera.main.WorldToViewportPoint(transform.position));

        context.HasDashed = true;
        _animationScript.SetTrigger(Configuration.AnimatorParameters.DashTrigger);

        StartCoroutine(DashWait(context));
        return direction.normalized * _dashSpeed;
    }

    private IEnumerator DashWait(MovementContext context)
    {
        _ghostTrail.ShowGhost();
        StartCoroutine(GroundDash(context));
        DOVirtual.Float(15f, 0, 0.35f, (float x) => context.Drag = x);

        _dashParticle.Play();
        UpdateDashState(0f, false, context);

        yield return new WaitForSeconds(.3f);

        _dashParticle.Stop();
        UpdateDashState(3f, true, context);
    }

    private void UpdateDashState(float gravityScale, bool jumpingEnabled, MovementContext context)
    {
        context .GravityScale = gravityScale;
        _betterJumping.enabled = jumpingEnabled;
        context.IsWallJumped = !jumpingEnabled;
        context.IsDashing = !jumpingEnabled;
    }

    private IEnumerator GroundDash(MovementContext context)
    {
        yield return new WaitForSeconds(.15f);
        if (context.Collision.OnGround)
            context.HasDashed = false;
    }
}
