using UnityEngine;

[RequireComponent(typeof(BetterJumping))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Fly), typeof(Run), typeof(Dash))]
[RequireComponent(typeof(WallSlide), typeof(Jump))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private bool _isGrounded;

    private Run _runS;
    private Fly _fly;
    private Dash _dash;
    private WallSlide _wallSlide;
    private Jump _jump;

    public MovementContext Context { get; private set; }

    #region Inspector values

    [SerializeField]
    private AnimationScript _animationScript;

    [SerializeField]
    private Collision _collision = null;

    public ParticleSystem JumpParticle;

    #endregion

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _runS = GetComponent<Run>();
        _fly = GetComponent<Fly>();
        _dash = GetComponent<Dash>();
        _wallSlide = GetComponent<WallSlide>();
        _jump = GetComponent<Jump>();

        Context = new MovementContext(_collision, _animationScript)
        {
            GravityScale = _rigidbody.gravityScale,
            Drag = _rigidbody.drag
        };
    }

    void Update()
    {
        float x = Input.GetAxis(Configuration.Input.HorizontalAxis);
        float y = Input.GetAxis(Configuration.Input.VerticalAxis);

        _rigidbody.velocity = _runS.Walk(x, _rigidbody.velocity, Context);
        _animationScript.SetHorizontalMovement(x, y, _rigidbody.velocity.y);
        //DoGrabWall();

        if (!_collision.OnWall || !Context.CanMove)
            Context.IsWallSliding = false;

        if (_collision.OnGround && !Context.IsDashing)
            Context.IsWallJumped = false;

        //UpdateGravityScale(y);
        _rigidbody.velocity = _fly.GetFlyVelocity(x, Context, _rigidbody.velocity);
        _rigidbody.velocity = _wallSlide.DoWallSlide(x, _rigidbody.velocity, Context);
        _rigidbody.velocity = _jump.DoJump(Context, _rigidbody.velocity);
        _rigidbody.velocity = _dash.DoDash(_rigidbody.velocity, Context);
        UpdateIsGrounded();

        if (!Context.IsWallSliding && Context.CanMove && x != 0)
        {
            Context.Side = x > 0 ? 1 : -1;
            _animationScript.Flip(Context.Side);
        }

        _rigidbody.gravityScale = Context.GravityScale;
        _rigidbody.drag = Context.Drag;
    }

    private void UpdateIsGrounded()
    {
        if (_collision.OnGround)
        {
            if (_isGrounded) return;
            JumpParticle.Play();
            Context.Side = _animationScript.Side;
            Context.HasDashed = false;
            Context.IsDashing = false;
            _isGrounded = true;
            return;
        }

        _isGrounded = false;
    }
}


//private void DoGrabWall()
//{
//    if (_collision.OnWall && Input.GetButton(Configuration.Input.WallClimbButton) && CanMove)
//    {
//        if (_side != _collision.WallSide)
//        {
//            _side *= -1;
//            _animationScript.Flip(_side);
//        }
//        IsWallGrabbing = true;
//        IsWallSliding = false;
//    }

//    if (Input.GetButtonUp(Configuration.Input.WallClimbButton) || !_collision.OnWall || !CanMove)
//        IsWallGrabbing = IsWallSliding = false;
//}

//private void UpdateGravityScale(float y)
//{

//    if (IsWallGrabbing && !IsDashing)
//    {
//        _rigidbody.gravityScale = 0;
//        float speedModifier = y > 0 ? WallClimbSpeedMultiplier : 1;
//        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, y * (RunSpeed * speedModifier));
//    }
//    else
//    {
//        _rigidbody.gravityScale = 3;
//    }
//}
